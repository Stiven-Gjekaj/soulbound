# Projectile management

Projectile management is, starting from 0.2.0, available from both the encounter and the
wave scripts. As a result it is now in its own section.

### `CreateProjectileLayer(string name, string position = "", boolean below = false)` [E/W]

Creates a layer named `name` that projectiles can be placed in. To create your new
projectile layer, you'll need to choose a pre-existing layer.

`position` must be the name of an existing projectile layer. The only default layer is
`""`, so **you must use it first**.

If `below` is true, the new layer will be created below the layer given in `position`,
otherwise it will be above it.

### `CreateProjectile(string spritename, number initial_x, number initial_y,` `string layer = "")` returns **bullet** [E/W]

Creates a bullet that you can store and modify, with its spawn position relative to the
center of the arena. The hitbox of the bullet is a rectangle around the sprite, unless you
use CYF's PP mode.

You can specify a layer if you want, otherwise the bullet will be in the normal
bullet layer.

### `CreateProjectileAbs(string spritename, number initial_x, number initial_y,` `string layer = "")` returns **bullet** [E/W]

Same as `CreateProjectile`, but the bullet's spawn position is relative to the bottom left
of the screen instead of the arena's center. The hitbox of the bullet is a rectangle around
the sprite, unless you use CYF's PP mode.

You can specify a layer if you want, otherwise the bullet will be in the normal
bullet layer.

## The Bullet object

This is what you use to move around the arena and store values in. You can store a bunch of
bullets in a table and modify them. The functions and variables you can use on a Bullet are
as follows.

New in v0.6.6: As this object exists in CYF's hierarchy, it's possible to manipulate its
parent and children. See the [General objects](objects/general-objects.md) page for more
information.

### **sprite** `Bullet.sprite`

The bullet's sprite component. See the [Sprites and animation](sprites-and-animation.md)
section for usage details.

Note: In 0.2.1a, modifying the sprite does not change the bullet's hitbox yet, it's always
the original square of the bullet when it was created.

But, in CYF, changing the bullet's sprite *does* modify the bullet's hitbox.

### **number** `Bullet.x`

The X position of this bullet, relative to the arena's center. A bullet at `x=0` and `y=0`
will be at the center of the arena.

### **number** `Bullet.y`

The Y position of this bullet, relative to the arena's center.

### **number** `Bullet.absx`

The X position of this bullet, relative to the bottom-left corner of the screen.

### **number** `Bullet.absy`

The Y position of this bullet, relative to the bottom-left corner of the screen.

### **boolean** `Bullet.ppcollision`

If this is true, the bullet will use the Pixel-Perfect Collision system.

By default, this is the encounter's **default** collision system.

Manually setting this will set `Bullet.ppchanged` to true.

See `SetPPCollision` in [The Pixel-Perfect Collision System](pixel-perfect-collision.md)
for more information.

### **boolean** `Bullet.ppchanged` (read-only)

Tells you if the bullet's collision system has been changed by manually changing
`Bullet.ppcollision`.

Bullets with `Bullet.ppchanged` set to true will NOT be affected by future calls of
`SetPPCollision` (see [The Pixel-Perfect Collision System](pixel-perfect-collision.md)).

Will be false after you call `Bullet.ResetCollisionSystem()`, or if you haven't changed
`Bullet.ppcollision`.

### `Bullet.ResetCollisionSystem()`

Resets the collision system of the bullet to the encounter's **default** collision system.

The default collision system is set by `SetPPCollision`. See
[The Pixel-Perfect Collision System](pixel-perfect-collision.md) for more information.

### **string** `Bullet.layer`

The bullet layer that the bullet is on.

Note: It is common practice to use `Bullet.layer` to deparent a bullet if you need to do
so. Setting it back to `""` will parent the bullet to its default layer, removing its
previous parenting altogether.

### **boolean** `Bullet.isactive` (read-only)

Used to check if the bullet is still active.

If the bullet has been removed, this will be `false`; otherwise `true`.

### `Bullet.Move(number x, number y)`

Move this bullet `x` pixels to the right and `y` pixels up.

A negative `x` will move it to the left, and a negative `y` will move it downwards.

### `Bullet.MoveTo(number x, number y)`

Move this bullet to this position immediately, relative to the arena's center.

### `Bullet.MoveToAbs(number x, number y)`

Move this bullet to this position immediately, relative to the bottom-left corner of the
screen.

### `Bullet.Remove()`

Destroys this bullet.

You can continue retrieving its `x`, `y`, `absx` and `absy` properties. Trying to move a
removed bullet will give you a Lua error. If you're not sure if your bullet still exists,
check `Bullet.isactive` first.

### `Bullet[string your_variable_name] = value` (OR) `Bullet.SetVar(string your_variable_name, value)`

Sets a variable on this bullet that you can retrieve with `Bullet.GetVar`.

Similar in use to `SetGlobal`, but you can use this to store specific variables on a
per-bullet basis.

### `Bullet[string your_variable_name]` (OR) `Bullet.GetVar(string your_variable_name)`

Gets a variable that you previously set using `Bullet.SetVar`.

### `Bullet.SendToTop()`

Moves this bullet on top of all currently existing projectiles.

Note that newly spawned projectiles are always on top by default; this function is mostly
to move existing bullets to the top.

Moves the bullet to the top of its current layer.

### `Bullet.SendToBottom()`

Moves this bullet below all currently existing projectiles.

Moves the bullet to the bottom of its current layer.

### `Bullet.isColliding()`

Returns true if the player is colliding with the bullet.

Will use PPCollision (pixel-perfect collisions) if the bullet has PP enabled. See
`Bullet.ppcollision`.

### `Bullet.OnHit(bullet bullet)`

This variable must receive a function, which can be done in two possible ways:

```lua
Bullet.OnHit = function(bullet)
    -- Your code
end
```

```lua
function Bullet.OnHit(bullet)
    -- Your code
end
```

If it has been assigned, this function will be run instead of the script's `OnHit` function
when the bullet collides with the Player.

The bullet and its `OnHit` function must both be created in the same script.

### **boolean** `Bullet.isPersistent = false`

Set this to true and, if you're not in retrocompatibility mode, this will make the bullet
stay loaded even after the wave ends.

## Examples

Here is an example of a bullet that chases you pretty fast, but slows down as it gets
closer. You have to keep moving to dodge it. This is a fairly basic example that makes use
of the Player object.

```lua
oursprite = "hOI!!!!"
--Create a new bullet, starting in the upper right corner.
chasingbullet = CreateProjectile(oursprite, Arena.width/2, Arena.height/2)
--Set initial speed of 0 in both directions.
chasingbullet.SetVar('xspeed', 0)
chasingbullet.SetVar('yspeed', 0)

function Update()
    -- Get the x/y difference between the player and bullet
    local xdifference = Player.x - chasingbullet.x
    local ydifference = Player.y - chasingbullet.y
    -- We create a new speed by first halving the original speed
    -- Then we add a small fraction of the position difference between the player and bullet.
    -- The result is a bullet that moves faster as it's further away, and slower when it's closer.
    -- The value we're dividing by is experimental. Experimenting with numbers is essential!
    local xspeed = chasingbullet.GetVar('xspeed') / 2 + xdifference / 100
    local yspeed = chasingbullet.GetVar('yspeed') / 2 + ydifference / 100
    -- Now move the bullet...
    chasingbullet.Move(xspeed, yspeed)
    -- ...and store our new speeds.
    chasingbullet.SetVar('xspeed', xspeed)
    chasingbullet.SetVar('yspeed', yspeed)
end
```

Below is an example of a fully scripted Wave using most of these functions. It will spawn a
projectile above the arena (assuming a width and height of 155/130), give it a random speed
in the X direction, and drop it downwards. If it hits the bottom border of the arena, it'll
bounce back up. Otherwise it'll continue falling off the screen.

```lua
spawntimer = 0
bullets = {}

-- This happens every frame while you're defending. --
function Update()
    spawntimer = spawntimer + 1 --Add 1 to the counter every frame

    -- This part takes care of bullet spawning. --
    if spawntimer%30 == 0 then  --This happens every 30 frames.
        local posx = 30 - math.random(60) --Set a random X position between -30 and 30
        local posy = 65 --and set the Y position to 65, on the top edge of the arena.
        local bullet = CreateProjectile('hOI!!!!', posx, posy) -- Create projectile with sprite hOI!!!!.png
        bullet.SetVar('velx', 1 - 2*math.random()) -- We'll use this for horizontal speed. Random between -1/1
        bullet.SetVar('vely', 0) -- We'll use this for fall speed. We're starting without downward movement.
        table.insert(bullets, bullet) -- Add this new Bullet object to the bullets table up there.
    end

    -- This part updates every bullet in the bullets table. --
    for i=1,#bullets do -- #bullets in Lua means 'length of bullets table'.
        local bullet = bullets[i] -- For convenience, so we don't have to use bullets[i]
        local velx = bullet.GetVar('velx') -- Get the X/Y velocity we just set
        local vely = bullet.GetVar('vely')
        local newposx = bullet.x + velx -- New position will be old position + velocity
        local newposy = bullet.y + vely
        if(bullet.x > -Arena.width/2 and bullet.x < Arena.width/2) then -- Are we inside the arena (horizontally)?
            if(bullet.y < -Arena.height/2 + 8) then -- And did we go past the bottom edge?
                bullet.MoveTo(bullet.x, -Arena.height/2 + 8) -- Don't move it past the edge!
                -- Note the +8; I know the bullet sprite I'm using is 16x16.
                -- Without adding 8 it'll be inside the edge.
                vely = 4 --reverse bounce direction
            end
        end
        vely = vely - 0.04 -- Apply gravity
        bullet.MoveTo(newposx, newposy) -- and finally, move our bullet
        bullet.SetVar('vely', vely) -- and store our new fall speed into the bullet again.
    end
end
```

### `OnHit(bullet bullet)`

Every time a bullet collides with a player, this function gets called on the script that
created the projectile. The bullet object in this function can be modified if you feel like
it. For more information on the bullet object, see the documentation above.

If you implement this function in your script, you have to manually define what should
happen after bullet collision. This is what allows you to create orange, cyan and green
projectiles, and much much more. If you don't implement this function in your wave script,
it'll stick to the default of dealing 3 damage on hit. Below are multiple examples of how
to use this function.

```lua
--Defining your own damage for this wave
function OnHit(bullet)
    Player.Hurt(10)
end
```

```lua
--Replicating cyan bullet functionality
function OnHit(bullet)
    if Player.isMoving then
        Player.Hurt(5)
    end
end
```

```lua
--Replicating orange bullet functionality; opposite condition of cyan
function OnHit(bullet)
    if not Player.isMoving then
        Player.Hurt(5)
    end
end
```

```lua
--Replicating green bullet functionality
function OnHit(bullet)
    Player.Heal(1)
    bullet.Remove()
end
```
