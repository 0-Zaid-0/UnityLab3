import zipfile
import re
import html
from pathlib import Path

candidates = [
    Path(r"C:\Users\Zaid's Laptop\Downloads\Lab 3(1).docx"),
    Path(r"C:\Users\ZAID'S~1\Downloads\Lab 3(1).docx"),
    Path(r"C:\Temp\Lab3.docx"),
]

src = None
for c in candidates:
    if c.exists():
        src = c
        break

if src is None:
    raise SystemExit("Could not find Lab 3(1).docx")

out_path = Path(__file__).resolve().parent / "lab3_extracted.txt"
with zipfile.ZipFile(src) as z:
    xml = z.read("word/document.xml").decode("utf-8")

text = re.sub(r"</w:p>", "\n", xml)
text = re.sub(r"<w:tab[^/]*/>", "\t", text)
text = re.sub(r"<[^>]+>", "", text)
text = html.unescape(text)
lines = [ln.rstrip() for ln in text.splitlines()]
out = "\n".join(lines)
while "\n\n\n" in out:
    out = out.replace("\n\n\n", "\n\n")
out = out.strip() + "\n"
out_path.write_text(out, encoding="utf-8")
print(f"Wrote {out_path}")
print(out)
