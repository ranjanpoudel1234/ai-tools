---
name: read-avro-files
description: Extracts and displays JSON data from Apache Avro files. Use this when the user wants to read, convert, or view the contents of an .avro file. Automatically deserializes nested JSON fields for better readability.
---

# Read Avro Files

## Overview
This skill helps extract JSON data from Apache Avro files and displays them in a readable format. It handles deserialization of nested byte strings and saves the output to a JSON file.

## When to Use This Skill
- User mentions they have an Avro file (.avro extension)
- User wants to convert Avro to JSON
- User wants to see the contents of an Avro file
- User provides a path to an .avro file

## Instructions

When this skill is invoked:

1. **Ask for the file path** if not provided by the user:
   - Use the AskUserQuestion tool to get the Avro file path
   - Ask: "Please provide the full path to the Avro file you want to convert"

2. **Ensure Python script exists**:
   - Check if `read_avro.py` exists in the current directory
   - If not, create it using the template below

3. **Install dependencies** (if needed):
   - Run: `pip install avro-python3`
   - Only install if the avro module is not already available

4. **Run the conversion**:
   - Execute: `python read_avro.py "<avro_file_path>"`
   - The script will:
     - Display each record as formatted JSON
     - Deserialize the Body field if it contains nested JSON
     - Save the output to a .json file in the same directory

5. **Present results**:
   - Show the user where the JSON output was saved
   - Summarize the number of records found
   - Highlight key information from the records if relevant

## Python Script Template

```python
import avro.schema
import avro.datafile
import avro.io
import json
import sys

def deserialize_body(record):
    """Deserialize the Body field if it's a byte string"""
    if 'Body' in record:
        body = record['Body']

        # Handle byte string representation
        if isinstance(body, str) and body.startswith("b'"):
            # Remove the b' prefix and trailing '
            json_str = body[2:-1]
            # Unescape the string
            json_str = json_str.encode().decode('unicode_escape')
            # Parse as JSON
            try:
                record['Body'] = json.loads(json_str)
            except json.JSONDecodeError as e:
                print(f"Warning: Could not parse Body as JSON: {e}")
        elif isinstance(body, bytes):
            # Handle actual bytes
            try:
                record['Body'] = json.loads(body.decode('utf-8'))
            except json.JSONDecodeError as e:
                print(f"Warning: Could not parse Body as JSON: {e}")

    return record

def read_avro_file(file_path):
    """Read an Avro file and output records as JSON"""
    try:
        with open(file_path, 'rb') as f:
            reader = avro.datafile.DataFileReader(f, avro.io.DatumReader())

            # Print each record as JSON
            record_count = 0
            all_records = []

            for record in reader:
                record_count += 1
                # Deserialize the Body field
                record = deserialize_body(record)
                all_records.append(record)
                print(f"\\n--- Record {record_count} ---")
                print(json.dumps(record, indent=2, default=str))

            reader.close()
            print(f"\\n\\nTotal records: {record_count}")

            # Optionally save to a file
            output_file = file_path.replace('.avro', '.json')
            with open(output_file, 'w') as out:
                json.dump(all_records, out, indent=2, default=str)
            print(f"\\nJSON output saved to: {output_file}")

    except FileNotFoundError:
        print(f"Error: File not found at {file_path}")
        sys.exit(1)
    except Exception as e:
        print(f"Error reading Avro file: {str(e)}")
        import traceback
        traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    if len(sys.argv) > 1:
        file_path = sys.argv[1]
    else:
        print("Usage: python read_avro.py <path_to_avro_file>")
        sys.exit(1)

    print(f"Reading Avro file: {file_path}\\n")
    read_avro_file(file_path)
```

## Expected Output Format

The script produces:
- Console output showing each record with formatted JSON
- A .json file saved in the same directory as the input file
- Record count summary

## Common Use Cases

1. **Event Hub captured data**: Avro files from Azure Event Hub captures containing event metadata and body
2. **Kafka messages**: Avro-serialized Kafka messages
3. **Data pipeline debugging**: Inspecting intermediate Avro files in data processing pipelines
4. **Schema validation**: Viewing actual data structure for schema comparison

## Notes

- The script handles nested JSON in byte string format (common in Event Hub captures)
- Dates and complex types are converted to strings in the output
- Large files will show all records in console but save efficiently to JSON
