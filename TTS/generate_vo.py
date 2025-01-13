import os
import sys
import json
from pathlib import Path

piper_dir = Path("./piper")
output_dir = Path("./output")

tts_model = "en_GB-cori-high"
#tts_model= "en_US-ryan-high"

def tts(text, name):
    os.system(f"echo {text} | {piper_dir / 'piper.exe'} -m {piper_dir / 'models' / tts_model}.onnx -f {output_dir / name}.wav")

if len(sys.argv) == 2:
    file_name = sys.argv[1]
    with open(f'{file_name}.json', 'r') as file:
        data = json.load(file)

    (output_dir / file_name).mkdir(exist_ok=True)

    name = data['name']
    texts = data['texts']
    
    for i in range(len(texts)):
        tts(texts[i], f"{file_name}/{name}-{i+1}")

else:
    print("Error")
