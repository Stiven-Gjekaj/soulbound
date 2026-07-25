# The Player object

### The Player object [E/M/W]

You can use this object to obtain information about the player. Since the player is always
16x16 pixels in Undertale, you can add or subtract 8 from the player's horizontal or
vertical position to get the edges of the player's hitbox, if you need that for anything.

- **number** `Player.x` (readonly) - gets the X position of the *player's center* relative
  to the arena's center.
- **number** `Player.y` (readonly) - get the Y position of the *player's center* relative
  to the arena's center.
- **number** `Player.absx` (readonly) - get the X position of the *player's center*
  relative to the bottom left corner of the screen.
- **number** `Player.absy` (readonly) - get the Y position of the *player's center*
  relative to the bottom left corner of the screen.
- **sprite** `Player.sprite` - the Player's soul sprite component. See the
  [Sprites and animation](../sprites-and-animation.md) section for usage details.
- **number** `Player.hp` - get or set the player's current HP. Can't exceed max HP. If set
  to 0, game over triggers.

  `<CYF>` The hp value is now a float, but there can be some problems about accuracy of
  float values.
- `<CYF>` **number** `Player.maxhp` - returns the player's MaxHP value.

  As of CYF v0.6.4, this is settable. Setting this is the same as calling
  `Player.SetMaxHPShift(<value>, 0, true, false, false)`. Max hp is not a float, but
  regular hp is.
- `<CYF>` **number** `Player.maxhpshift` (readonly) - returns the difference between the
  player's current Max HP and their normal, unmodified Max HP.
- `<CYF>` **number** `Player.atk` - base attack of the player. Depends on the player's
  level.

  Changing `Player.lv` or ending the battle will **reset** this to its intended value.
- `<CYF>` **string** `Player.weapon` (readonly) - name of the player's current weapon.
- `<CYF>` **number** `Player.weaponatk` (readonly) - attack value of the player's current
  weapon.
- `<CYF>` **number** `Player.def` - base defense of the player. Depends on the player's
  level.

  Changing `Player.lv` or ending the battle will **reset** this to its intended value.
- `<CYF>` **string** `Player.armor` (readonly) - name of the player's current armor.
- `<CYF>` **number** `Player.armordef` (readonly) - defense value of the player's current
  armor.
- `<CYF>` **number** `Player.speed` - Player's speed in pixels per second. Default is 120.
- **string** `Player.name` - get or set the player's current name.

  `<0.2.1a>` 6 letters max. By default, it is a name randomly chosen from a small list of
  pre-set names.

  `<CYF>` 9 letters max. "Rhenao" by default.
- **number** `Player.lv` - use this to get or set the player's current level. It can be
  between 1 and 20 (99 in `<CYF>`). It's 1 by default. Player starts with 20HP / 10 ATK and
  gets 4 HP / 2 ATK per level.

  Leveling up the player through code doesn't automatically heal them; you'll have to do
  this manually.
- `<CYF>` **number** `Player.lastenemychosen` (readonly) - gets the id of the last chosen
  enemy in the ACT/FIGHT menus. -1 at the beginning of a fight.

  Note: This is NOT the same as the enemy's position in the `enemies` table. This is the
  position of the enemies in the *menu* in-game.
- `<CYF>` **number** `Player.lasthitmultiplier` (readonly) - gets the accuracy value from
  the last time the player was in `ATTACKING`. Normally, it will be between 0 and 2. It
  will be -1 if the player missed the attack (and at the beginning of the battle) and 2.2
  if the attack was perfectly precise.
- **boolean** `Player.ishurting` (readonly) - true if the player's currently blinking due
  to being hurt, false otherwise.
- **boolean** `Player.ismoving` (readonly) - true if the player is currently moving in
  battle, false otherwise. Will always be false while not in a wave script.

  Note: This only will change with the default control scheme (see
  `Player.SetControlOverride` below).
- `Player.Hurt(number damage, number invul_time = 1.7, boolean ignoreDef = false, boolean playSound = true)` -
  deals damage to the player, and makes them invincible for `invul_time` seconds. Can not
  hurt the player again if they are already hurting (flashing).

  Set `ignoreDef` to true, and if the encounter variable `allowPlayerDef` is true, the
  damage dealt here will ignore the player's defense.

  `<CYF>` Call `Player.Hurt(0, 0)` to stop the player's invincibility frames without
  making a sound. Set `playSound` to `false`, and no sound will be played whatsoever.
- `Player.Heal(number value)` - heals the player for this amount. This is exactly the same
  as `Player.Hurt(-value, 0)`. It also plays the healing sound.
- `Player.SetControlOverride(boolean boolean)` - either true or false. Only usable in
  waves. If true, this will prevent the player soul from doing its regular movement and
  keyboard control for the rest of the wave, in other words it disables the red soul
  functionality.

  Use this if you want to implement your own controls in a wave, for example a custom soul
  mode like the blue or purple soul.
- `<CYF>` `Player.Move(number x, number y, boolean ignoreWalls = false)` - Moves the player
  soul based on its last position. If `ignoreWalls` is false, it will make sure the player
  doesn't go outside of the arena; otherwise, it ignores the arena's boundaries. If you
  want to move the player out of bounds in a wave, you'll have to call
  `Player.SetControlOverride(true)`, as the player's default movement keeps the soul inside
  the arena otherwise.
- `Player.MoveTo(number x, number y, boolean ignoreWalls = false)` - Moves the player soul
  to this position relative to the arena's center. If `ignoreWalls` is false, it will make
  sure the player doesn't go outside of the arena; otherwise, it ignores the arena's
  boundaries. If you want to move the player out of bounds in a wave, you'll have to call
  `Player.SetControlOverride(true)` as the player's default movement keeps the soul inside
  the arena.
- `Player.MoveToAbs(number x, number y, boolean ignoreWalls = false)` - Moves the player
  soul to this position relative to the bottom left of the screen. If `ignoreWalls` is
  false, it will make sure the player doesn't go outside of the arena; otherwise, it
  ignores the arena's boundaries. If you want to move the player out of bounds in a wave,
  you'll have to call `Player.SetControlOverride(true)` as the player's default movement
  keeps the soul inside the arena.
- `<CYF>` `Player.ForceHP(number amount)` - Lets you set the player's HP to a number above
  the player's Max HP. The REAL maximum is 999.
- `<CYF>` `Player.SetMaxHPShift(number shift, number invulnerabilitySeconds = 1.7f, boolean set = false, boolean canHeal = false, boolean playSound = true)` -
  Lets you set the Player's MaxHP, relative to their normal MaxHP. If the total MaxHP is
  negative or nil, it causes a GameOver. The max value for the Player's MaxHP is 999.

  Formula: Player's MaxHP = Basis MaxHP + MaxHP Shift

  - `shift` = The value that will be added to the Max HP. If `set` is true, this will be
    the player's *new* Max HP.
  - `invulnerabilitySeconds` = The time for which the player will be invulnerable after
    this operation.
  - `set` = true if you want to set the Player's MaxHP directly.
  - `canHeal` = true if the player will automatically gain any health added by this
    function as normal (yellow) health. Does not apply for losing health.
  - `playSound` = true if you want to play the heal or hurt sound when the operation is
    done.
- `<CYF>` `Player.ResetStats(boolean resetMHP = true, boolean resetATK = true, boolean resetDEF = false)` -
  Resets the player's Max HP, ATK and/or DEF to their original values based on the player's
  LV.

  You can choose which stats to reset through the arguments:

  - `resetMHP` = If true, the player's Max HP will be set back to what it should be based
    on the player's LV.
  - `resetATK` = If true, the player's ATK will be set back to what it should be based on
    the player's LV.
  - `resetDEF` = If true, the player's DEF will be set back to what it should be based on
    the player's LV.

  The values set through this function are chosen according to
  [this chart](https://undertale.fandom.com/wiki/Stats#Stat_Chart).
- `<CYF>` `Player.SetAttackAnim({table of string} anim, number frequency, string prefix = "")` -
  Sets the animation used when attacking an enemy. Use it like `sprite.SetAnimation()`.
- `<CYF>` `Player.ResetAttackAnim()` - Resets the animation of the player's attack to the
  default slashing animation.
- `<CYF>` `Player.ChangeTarget(number targetNumber)` - Changes the target of the Player's
  attack while in `ATTACKING`. Does nothing outside of the state `ATTACKING`. It should be
  used in `BeforeDamageCalculation()` or `BeforeDamageValues()`.
- `<CYF>` `Player.ForceAttack(number enemyID, number damage = normal_damage_values)` -
  Forces an attack towards the enemy number `enemyID`. If you want to, you can choose the
  attack's damage, too, but otherwise damage will be calculated normally.

  If you want enemies to be able to die this way, you MUST check with `Player.CheckDeath`.
- `<CYF>` `Player.MultiTarget({table of number} targetIDs = all, {table of number} OR number damage = normal_damage_values)` -
  The next attack the player executes will attack the enemies contained in `targetIDs` and
  deal `damage` damage. If you want to, you can choose the attack's damage, too, but
  otherwise damage will be calculated normally. If you put nothing as a parameter, this
  will attack all enemies with the normal damage calculations.

  Each target needs to have one damage value if you use a `table of numbers` for the
  `damage` value, or all targets can share the same damage value if the `damage` value is
  a simple `number`.
- `<CYF>` `Player.MultiTarget(number damage)` - The next attack the player executes will
  attack all enemies and deals `damage` damage.
- `<CYF>` `Player.ForceMultiAttack({table of number} targetIDs = all, {table of number} OR number damage = normal_damage_values)` -
  Forces an attack towards the enemies contained in `targetIDs` and deals `damage` damage.
  If you want to, you can choose the attack's damage, too, but otherwise damage will be
  calculated normally. If you put nothing as a parameter, this will attack all enemies with
  the normal damage calculations.

  Each target needs to have one damage value if you use a `table of numbers` for the
  `damage` value, or all targets can share the same damage value if the `damage` value is
  a simple `number`.

  If you want enemies to be able to die this way, you MUST check with `Player.CheckDeath`.
- `<CYF>` `Player.ForceMultiAttack(number damage)` - Forces an attack that deals `damage`
  damage to all enemies.

  If you want enemies to be able to die this way, you MUST check with `Player.CheckDeath`.
- `<CYF>` `Player.CheckDeath()` - Checks if the enemies are dead after a forced attack.
  Enemies are not killed by forced attacks because death texts are easier to implement if
  you use `ForceAttack` or `ForceMultiAttack` in the middle of text.
