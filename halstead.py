import math
from pathlib import Path
from collections import Counter
from pygments import lex
from pygments.lexers import CSharpLexer
from pygments.token import Token

ROOT = Path(".")
NEANALIZUOJAMI_APLANKAI = {"bin", "obj", ".git", ".vs"}

failai = [
    p for p in ROOT.rglob("*.cs")
    if not any(dalis in NEANALIZUOJAMI_APLANKAI for dalis in p.parts)
]

operatoriai = Counter()
operandai = Counter()

for failas in failai:
    kodas = failas.read_text(encoding="utf-8", errors="replace")

    for tokeno_tipas, reiksme in lex(kodas, CSharpLexer()):
        if tokeno_tipas in Token.Comment or tokeno_tipas in Token.Text:
            continue

        if tokeno_tipas in Token.Punctuation:
            if reiksme in {"(", ")", "{", "}", "[", "]", ";", ","}:
                operatoriai[reiksme] += 1
        elif tokeno_tipas in Token.Operator:
            operatoriai[reiksme] += 1
        elif tokeno_tipas in Token.Keyword:
            operatoriai[reiksme] += 1
        elif tokeno_tipas in Token.Name:
            operandai[reiksme] += 1
        elif tokeno_tipas in Token.Literal:
            operandai[reiksme] += 1

n1 = len(operatoriai)
n2 = len(operandai)
N1 = sum(operatoriai.values())
N2 = sum(operandai.values())

zodynas = n1 + n2
ilgis = N1 + N2
apimtis = ilgis * math.log2(zodynas) if zodynas > 1 else 0
sunkumas = (n1 / 2) * (N2 / n2) if n2 else 0
pastangos = sunkumas * apimtis
programos_gylio_ivertis = 1 / sunkumas if sunkumas else 0
klaidu_ivertis = apimtis / 3000

print("HALSTEAD METRIKŲ ANALIZĖ")
print("=" * 40)
print(f"Išanalizuotų C# failų skaičius: {len(failai)}")
print(f"Skirtingų operatorių skaičius (n1): {n1}")
print(f"Skirtingų operandų skaičius (n2): {n2}")
print(f"Bendras operatorių skaičius (N1): {N1}")
print(f"Bendras operandų skaičius (N2): {N2}")
print(f"Žodynas (n): {zodynas}")
print(f"Programos ilgis (N): {ilgis}")
print(f"Apimtis (V): {apimtis:.2f}")
print(f"Sunkumas (D): {sunkumas:.2f}")
print(f"Pastangos (E): {pastangos:.2f}")
print(f"Programos gylio įvertis (L = 1/D): {programos_gylio_ivertis:.6f}")
print(f"Numatomas klaidų skaičius (B = V/3000): {klaidu_ivertis:.4f}")
print()
print("Pastaba: rezultatai priklauso nuo operatorių ir operandų klasifikavimo.")
print("Programos gylio ir klaidų skaičiaus formulės yra apytikslės.")
