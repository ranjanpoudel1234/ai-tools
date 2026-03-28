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
                print(f"\n--- Record {record_count} ---")
                print(json.dumps(record, indent=2, default=str))

            reader.close()
            print(f"\n\nTotal records: {record_count}")

            # Optionally save to a file
            output_file = file_path.replace('.avro', '.json')
            with open(output_file, 'w') as out:
                json.dump(all_records, out, indent=2, default=str)
            print(f"\nJSON output saved to: {output_file}")

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

    print(f"Reading Avro file: {file_path}\n")
    read_avro_file(file_path)
