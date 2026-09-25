import type { Plugin } from "@opencode-ai/plugin"
import { tool } from "@opencode-ai/plugin"
import { writeFileSync } from "node:fs"

const DEBUG_LOG = "C:\\Users\\261906\\.config\\opencode\\plugins\\self-compact.debug.log"
function debugLog(label: string, data: unknown) {
  try {
    writeFileSync(
      DEBUG_LOG,
      `[${new Date().toISOString()}] ${label}\n${JSON.stringify(data, null, 2)}\n\n`,
      { flag: "a" }
    )
  } catch {
    // ignore
  }
}

/**
 * Self-Compact Plugin
 *
 * Port of IndyDevDan's "Self Compact Pi Agent" concept to OpenCode.
 * https://youtu.be/3b0U4_02bAE / https://github.com/disler/self-compact-pi-agent
 *
 * Gives the agent self-awareness of its own context window usage and a
 * voluntary `self_compact` escape hatch, with a hard forced cutoff as a
 * safety net if the agent forgets. See:
 * C:\Users\261906\.config\opencode\tasks\self-compact-plugin.md
 */

type ThresholdOption = string | number | undefined

type Thresholds = {
  notice: number // fraction 0-1
  warning: number
  forced: number
}

type SessionState = {
  usedTokens: number
  contextWindow: number
  percent: number
  lastNoticeShown: boolean
  lastWarningShown: boolean
  pendingNote: string | null
}

const DEFAULT_THRESHOLDS: Thresholds = { notice: 0.10, warning: 0.20, forced: 0.30 }
const NOTE_MAX_CHARS = 24_000

/** Parse a threshold value: "10%" -> 0.10, "50k" -> 50000 (raw token count,
 * resolved to a fraction later once context window is known), "0.1" -> 0.1,
 * plain number -> treated as raw token count if > 1, else fraction. */
function parseThresholdRaw(value: ThresholdOption): { fraction?: number; tokens?: number } | undefined {
  if (value === undefined) return undefined
  if (typeof value === "number") {
    return value > 1 ? { tokens: value } : { fraction: value }
  }
  const s = value.trim().toLowerCase()
  if (s.endsWith("%")) {
    const n = parseFloat(s.slice(0, -1))
    return { fraction: n / 100 }
  }
  if (s.endsWith("k")) {
    return { tokens: parseFloat(s.slice(0, -1)) * 1_000 }
  }
  if (s.endsWith("m")) {
    return { tokens: parseFloat(s.slice(0, -1)) * 1_000_000 }
  }
  const n = parseFloat(s)
  if (Number.isNaN(n)) return undefined
  return n > 1 ? { tokens: n } : { fraction: n }
}

function resolveThreshold(raw: ReturnType<typeof parseThresholdRaw>, contextWindow: number, fallback: number): number {
  if (!raw) return fallback
  if (raw.fraction !== undefined) return raw.fraction
  if (raw.tokens !== undefined) return raw.tokens / contextWindow
  return fallback
}

export const SelfCompactPlugin: Plugin = async ({ client }, options) => {
  const opts = (options ?? {}) as { notice?: ThresholdOption; warning?: ThresholdOption; forced?: ThresholdOption }

  const rawNotice = parseThresholdRaw(opts.notice)
  const rawWarning = parseThresholdRaw(opts.warning)
  const rawForced = parseThresholdRaw(opts.forced)

  const state = new Map<string, SessionState>()

  function getState(sessionID: string): SessionState {
    let s = state.get(sessionID)
    if (!s) {
      s = {
        usedTokens: 0,
        contextWindow: 0,
        percent: 0,
        lastNoticeShown: false,
        lastWarningShown: false,
        pendingNote: null,
      }
      state.set(sessionID, s)
    }
    return s
  }

  function thresholdsFor(contextWindow: number): Thresholds {
    return {
      notice: resolveThreshold(rawNotice, contextWindow, DEFAULT_THRESHOLDS.notice),
      warning: resolveThreshold(rawWarning, contextWindow, DEFAULT_THRESHOLDS.warning),
      forced: resolveThreshold(rawForced, contextWindow, DEFAULT_THRESHOLDS.forced),
    }
  }

  async function refreshUsage(sessionID: string, contextWindow: number): Promise<SessionState> {
    const s = getState(sessionID)
    s.contextWindow = contextWindow || s.contextWindow
    try {
      const res = await client.session.messages({ path: { id: sessionID } })
      debugLog("session.messages raw", res)
      const messages = (res as any)?.data ?? (res as any) ?? []
      debugLog("messages array length", { length: messages?.length, sample: messages?.[messages.length - 1] })
      for (let i = messages.length - 1; i >= 0; i--) {
        const info = messages[i]?.info ?? messages[i]
        if (info?.role === "assistant" && info?.tokens) {
          const t = info.tokens
          s.usedTokens = (t.input ?? 0) + (t.cache?.read ?? 0) + (t.cache?.write ?? 0) + (t.reasoning ?? 0)
          break
        }
      }
    } catch (e) {
      debugLog("refreshUsage error", { error: String(e) })
      // best-effort; keep last known usage if fetch fails
    }
    s.percent = s.contextWindow > 0 ? s.usedTokens / s.contextWindow : 0
    return s
  }

  return {
    "chat.params": async (input) => {
      const contextWindow = input.model?.limit?.context ?? 0
      await refreshUsage(input.sessionID, contextWindow)
    },

    "experimental.chat.system.transform": async (input, output) => {
      if (!input.sessionID) return
      const contextWindow = input.model?.limit?.context ?? 0
      const s = await refreshUsage(input.sessionID, contextWindow)
      const t = thresholdsFor(s.contextWindow)

      if (s.percent >= t.forced) {
        // Forced messaging handled primarily by tool.execute.before block;
        // still remind here since system transform runs every turn.
        output.system.push(
          `[self-compact] CONTEXT FORCED CUTOFF: ${(s.percent * 100).toFixed(1)}% of context window used. ` +
          `All tools except self_compact are blocked. Call self_compact({note_to_self}) now with a handoff note ` +
          `covering: Goal, Progress (Done/In Progress/Blocked), Key Decisions, Next Steps, Critical Context ` +
          `(files read/modified, exact paths/commands/errors). Do not invent completed work.`
        )
      } else if (s.percent >= t.warning && !s.lastWarningShown) {
        s.lastWarningShown = true
        output.system.push(
          `[self-compact] Context warning: ${(s.percent * 100).toFixed(1)}% of context window used ` +
          `(forced cutoff at ${(t.forced * 100).toFixed(0)}%). Wrap up the current unit of work and call ` +
          `self_compact({note_to_self}) soon at a clean stopping point.`
        )
      } else if (s.percent >= t.notice && !s.lastNoticeShown) {
        s.lastNoticeShown = true
        output.system.push(
          `[self-compact] Context notice: ${(s.percent * 100).toFixed(1)}% of context window used. ` +
          `No action needed yet; call view_context() any time to check usage.`
        )
      }
    },

    "tool.execute.before": async (input) => {
      const s = state.get(input.sessionID)
      if (!s) return
      const t = thresholdsFor(s.contextWindow)
      if (s.percent >= t.forced && input.tool !== "self_compact" && input.tool !== "view_context") {
        throw new Error(
          `[self-compact] Forced context cutoff reached (${(s.percent * 100).toFixed(1)}%). ` +
          `All tools are blocked except self_compact. Call self_compact({note_to_self}) with your handoff note now.`
        )
      }
    },

    "experimental.session.compacting": async (input, output) => {
      const s = state.get(input.sessionID)
      if (s?.pendingNote) {
        output.context.push(
          `--- SELF-COMPACT HANDOFF NOTE (verbatim, written by the agent before compaction) ---\n` +
          s.pendingNote +
          `\n--- END HANDOFF NOTE ---`
        )
        s.pendingNote = null
        s.lastNoticeShown = false
        s.lastWarningShown = false
      }
    },

    tool: {
      view_context: tool({
        description:
          "View current context window usage: tokens used, context window size, percent used, and the " +
          "notice/warning/forced thresholds. Use this any time to check how much context budget remains.",
        args: {},
        async execute(_args, context) {
          const prior = getState(context.sessionID)
          const s = await refreshUsage(context.sessionID, prior.contextWindow)
          const t = thresholdsFor(s.contextWindow)
          return JSON.stringify(
            {
              used_tokens: s.usedTokens,
              context_window: s.contextWindow,
              percent: Number((s.percent * 100).toFixed(2)),
              remaining_tokens: Math.max(0, s.contextWindow - s.usedTokens),
              notice_at_percent: Number((t.notice * 100).toFixed(1)),
              warning_at_percent: Number((t.warning * 100).toFixed(1)),
              forced_at_percent: Number((t.forced * 100).toFixed(1)),
            },
            null,
            2
          )
        },
      }),

      self_compact: tool({
        description:
          "Voluntarily compact the session's context window. Write a handoff note capturing your current " +
          "goal, progress (done/in progress/blocked), key decisions, next steps, and critical context " +
          "(exact file paths, commands, errors read/modified) so no intent is lost. The note is stored " +
          "verbatim and re-injected immediately after compaction as the next message. Never invent " +
          "completed work. Call this voluntarily before you hit the forced cutoff, or immediately when " +
          "forced-cutoff is announced.",
        args: {
          note_to_self: tool.schema.string().max(NOTE_MAX_CHARS).describe(
            "Handoff note: Goal / Progress (Done, In Progress, Blocked) / Key Decisions / Next Steps / " +
            "Critical Context (files read, files modified, exact paths/commands/errors)."
          ),
        },
        async execute(args, context) {
          const s = getState(context.sessionID)
          s.pendingNote = args.note_to_self.slice(0, NOTE_MAX_CHARS)
          try {
            await client.session.summarize({ path: { id: context.sessionID } })
          } catch (e) {
            return `Note stored but compaction call failed: ${String(e)}. Note will still be re-injected on next compaction.`
          }
          return "Handoff note stored and compaction triggered. Your note will be re-injected verbatim after compaction completes."
        },
      }),
    },
  }
}

export default SelfCompactPlugin
