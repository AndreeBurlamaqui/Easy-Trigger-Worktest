# Worktest for Easy Trigger's studio. [Made in 2 days]

#### Prompt: 
"Your task is to create life in the scene with the player and enemies, try your very best to get a nice game feel to player jump and movements. Other than that, your hands are free. Let’s see what you can come up with."

#### Objective: 
- Create a simple gameplay style that is driven by the animation limits. I.e., no shooting while jumping (because no animation). Following by systems that are modular and split in each script.
- Attack Module: Any unit can use it, and will be used to either shoot or punch. Each shoot costs an ammo, and when 0, the next shooting attempt will count as a reloading.
- Health Module: Any unit can use it (even objects) and will be used to see tags as "damageable". Send events when hit and when diying. Can also set dropables when dying (with 0 to 100 percentage chance).
- Movement Module: Any unit can use it. The gravity is handled by Rigibody2D, but the actual movement, ground check and such is handled by this module.
- Stealth Module: Any unit can use it. On certain areas, you can press Z to hide. You'll not be seen by any other unit and if you try to shoot, you'll stop being hidden. You cannot move while hidden.
- InstaPunch Module: Only player can use it. Overdrive the punch when the enemy is at 1 life. Will move the unit to the target and apply a deathly blow. This is heavily inspired by DOOM. When killing someone with this, the player will get a free (and fast) reload.
- Enemy behaviours are basic. One Patrol Behaviour that will go forward and turn when there's no ground or a wall in front of it. And one Sight Behaviour that, based of a boxy line-of-sight, will tell when someone of the target layer is inside that.

#### Animations:
###### Player:
- Walk (0-5) \n
- FrontDeath (6-9)
- BackDeath(5/10-13)
- Shoot (14-17)
- Punch (17-20)
- Jump (26)
- Fall (27)
- Duck (28)
- Hide (29)
- Idle (22-25)

###### Tomahawk (Enemy):
- Walk (0-5)
- Shoot (14-17)
- Hit (29)
- Idle (22-25)
- FrontDeath (6-9)

#### Things I'd improve (that's not already written in comments):
- Add a time to warn player that the enemy is going to shoot, like MGS " ! "
- Backstab extra damage, to help stealth playstyle
- More visual indication that it's reloading
- Sounds, I just kept the pistol one
- Jump curve
- Fix bugs specially about the one way 

#### Observations:
Events doesn't work as expected, because Listeners script should show an UnityEvent, unfortunate, if it's not the Simple Event, it won't show on this Unity version. It's a known and fixed bug on after versions.

#### Known Bugs:
- Sometimes when doing going up and down too fast on one-ways the physics will break and it'll count as you're not grounded
- Some visual bugs from animations depending on how the interactions are updating it 

## Gameplay Video:

https://user-images.githubusercontent.com/87104166/188931145-1c4238e3-ec01-4df2-ba3a-9db5c11f21f9.mp4



