import re
import json

# Đọc nội dung từ file input.txt
with open('input.txt', 'r', encoding='utf-8') as file:
    text = file.read()

# Tách câu theo số thứ tự đầu câu (loại bỏ số)
verses = re.split(r'\d+(?=[^\d])', text)
verses = [verse.strip() for verse in verses if verse.strip()]

# Chuyển thành JSON
json_output = json.dumps(verses, ensure_ascii=False, indent=2)

# Ghi nội dung JSON ra file output.txt
with open('output.txt', 'w', encoding='utf-8') as file:
    file.write(json_output)
