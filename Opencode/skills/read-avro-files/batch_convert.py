import os
import glob
import subprocess
import sys

def batch_convert_avro_files(directory):
    """Convert all Avro files in a directory to JSON"""

    # Find all .avro files
    pattern = os.path.join(directory, "*.avro")
    avro_files = glob.glob(pattern)

    if not avro_files:
        print(f"No .avro files found in {directory}")
        return

    print(f"Found {len(avro_files)} Avro files to convert")
    print("=" * 60)

    success_count = 0
    error_count = 0
    empty_count = 0
    total_records = 0

    for i, file_path in enumerate(sorted(avro_files), 1):
        filename = os.path.basename(file_path)
        print(f"\n[{i}/{len(avro_files)}] Processing: {filename}")

        try:
            # Run the read_avro.py script
            result = subprocess.run(
                ["python", "read_avro.py", file_path],
                capture_output=True,
                text=True,
                timeout=30
            )

            # Parse output for record count
            for line in result.stdout.split('\n'):
                if "Total records:" in line:
                    count = int(line.split("Total records:")[1].strip())
                    total_records += count
                    if count == 0:
                        empty_count += 1
                        print(f"  ✓ Converted (empty)")
                    else:
                        print(f"  ✓ Converted ({count} records)")
                    success_count += 1
                    break

        except subprocess.TimeoutExpired:
            print(f"  ✗ Timeout")
            error_count += 1
        except Exception as e:
            print(f"  ✗ Error: {str(e)}")
            error_count += 1

    print("\n" + "=" * 60)
    print("SUMMARY")
    print("=" * 60)
    print(f"Total files processed:     {len(avro_files)}")
    print(f"Successful conversions:    {success_count}")
    print(f"Empty files:               {empty_count}")
    print(f"Errors:                    {error_count}")
    print(f"Total records extracted:   {total_records}")
    print(f"\nJSON files saved to:       {directory}")

if __name__ == "__main__":
    if len(sys.argv) > 1:
        directory = sys.argv[1]
    else:
        print("Usage: python batch_convert.py <directory>")
        sys.exit(1)

    batch_convert_avro_files(directory)
