import os
import shutil

base = os.path.dirname(os.path.abspath(__file__))
art = os.path.join(base, "Assets", "StarDefender", "Art")
sd = os.path.join(base, "Assets", "StarDefender", "Resources", "SD")
os.makedirs(sd, exist_ok=True)

files = [
    ("Player", "playerShip1_blue.png"),
    ("Enemies", "enemyRed1.png"),
    ("Enemies", "enemyRed2.png"),
    ("Enemies", "enemyRed3.png"),
    ("Enemies", "ufoRed.png"),
    ("Lasers", "laserBlue01.png"),
    ("Lasers", "laserGreen01.png"),
    ("Lasers", "laserRed01.png"),
    ("Background", "darkPurple.png"),
    ("Background", "black.png"),
    ("UI", "playerLife1_blue.png"),
    ("UI", "powerupYellow_bolt.png"),
    ("UI", "powerupYellow_star.png"),
]

for sub, name in files:
    src = os.path.join(art, sub, name)
    dst = os.path.join(sd, name)
    if os.path.isfile(src):
        shutil.copy2(src, dst)
        print("copied", name)
    else:
        print("MISSING", src)

print("---SD---")
for name in sorted(os.listdir(sd)):
    print(name)
