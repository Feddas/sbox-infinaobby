# TODO
1. Change procedural level generation to be based on how many seconds a player jump lasts for, from leaving ground to touching ground again. Use that seconds value to change the length of the platform cubes and their interval. Currently all platform cubes are the same scale, spawned at an increasing interval. The interval was guesstimated from trial and error.

# UI

3 values from `Code/Scores.cs` bind to `Code/ui/UiHud.razor`; MaxHeight, MaxScore, & Instructions

# Achievements

Are handled by `Code/ScoreAchievement.cs`

| name | Display Name |
|---|---|
| multi_box | multi box - reach 400 height |

## doc
1. https://sbox.game/feddas/infinaobby/service/achievements/add
1. https://services.facepunch.com/sbox/achievement/list?package=feddas.infinaobby

# Architecture

Player is restricted to 2D movement. Enforced by `Player.cs`'s `wishVelocity.x = 0;`
This is enabled by using the old PlayerCharacter.cs.

Updated PlayerController.cs's [WishVelocity](https://github.com/Facepunch/sbox-public/blob/master/engine/Sandbox.Engine/Scene/Components/Game/PlayerController/PlayerController.Input.cs#L88).x = 0 [inside of OnFixedUpdate()](https://github.com/Facepunch/sbox-public/issues/2777). Changed incoming.prefab friction to 0.

# Publish

1. Open InfinaObby in s&Box Editor
2. Click "Infina-Obby" in the top right. Just to the left of the editor windows close/minimize/maximize buttons.
3. On the dropdown that appears, select "Publish...".
4. On the new window that pops up. Click "Next" then "Upload Files".
5. Enter some Change Detail.
6. Click "Publish New Revision" then "Finished".

SubZero's tips on s&box publishing https://steamcommunity.com/sharedfiles/filedetails/?id=3595903475