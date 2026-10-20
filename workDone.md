#### Part 1 — Prototype 4 (island game)

I finished the Unit 4 gameplay stuff (roll around, camera, enemies, waves, powerup).

The extra lab bit: normally the powerup turns on the second you touch it. I changed that. You pick it up → it’s stored → you press Fire (Space) → *then* it activates. That uses the new Input System and a C# event (`Attack.performed`), so the fire button “tells” the player script to turn the powerup on. Indicator shows you’ve got it; knockback only works after you fire.

---

#### Part 2 

Input  
 I set up `GameInput` — one place that reads Move + Fire from the new Input System and fires C# events. Other scripts just listen. No messy “check keys in every script.”

Player ship

- Move left/right along the bottom
- Can’t leave the screen (clamped)
- Shoot with Space / click

Combat

- Your bullets damage invaders
- Invader dies after 2 hits (or 1 if weapon upgrade is on)
- You get points when they die

Lives / death

- 3 lives
- Hit by enemy bullet/body → lose a life
- Respawn with a 2 second flash = invulnerable so you don’t die instantly again

Powerups

- Yellow boxes drop sometimes
- Grab one → gun upgrades for 10 seconds (faster shots, stronger)

HUD

- Always shows Score and Lives on screen
- Game over text + press R to restart

Audio

- Sound when you shoot
- Sound when you lose a life
- (Kill sound plays when an enemy dies too)

Art

- Wired in free Kenney space-shooter sprites so it’s not just plain cubes

So in short: I own how the player plays the game/ controls, shooting, surviving, UI, powerups, and the Part 1 “fire to activate powerup” feature.

---

#### teammate work

- Wave 2 circular invaders
- Body-collision kill on player
- Free-roaming dangerous invader
- Angled downward shots
- testing / polish
