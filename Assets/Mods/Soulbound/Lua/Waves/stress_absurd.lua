-- The absurd turn: enough bullets to make the machine miss steps rather than merely slow
-- down. Everything before it held close to sixty frames a second, so the catch-up ceiling
-- in BattleTick was never really tested and what happens past it was still argument.
--
-- Same shape as the other stress waves: exactly TARGET Update calls, then it ends itself,
-- so the turn's length is a number of battle steps rather than a number of seconds. It
-- reports the same line.
--
-- The number that matters here is d. Above zero means a frame took long enough to owe more
-- steps than the ceiling allows and the fight threw the excess away rather than repaying
-- it. When that happens w climbs far above g: at most five steps run per frame, so six
-- hundred steps need at least a hundred and twenty frames however slow each one is.
--
-- BULLETS is meant to be turned up. If a run reports d0 the load was not heavy enough and
-- the honest response is to double it and run again, not to conclude anything.
local BULLETS = 1500
local TARGET  = 600         -- ten seconds of game time at sixty steps a second

-- Creation is spread over the first SPAWN_STEPS updates rather than done in one. Fifteen
-- hundred CreateProjectile calls in a single frame is a stall measured in seconds, and it
-- would land in worstMult and describe the spawn rather than the load it creates.
local SPAWN_STEPS = 60
local PER_STEP    = math.ceil(BULLETS / SPAWN_STEPS)

-- Which turn this is, set by the encounter before the wave is compiled. Floored and
-- defaulted because it reaches Lua as a number and is used as an integer.
local SLOT    = math.floor(tonumber(GetGlobal("stress_slot")) or 4)

local updates   = 0
local startTime = -1
local worstMult = 0
local startDrop = 0
local spawned   = 0
local bullets   = {}

function Update()
    if startTime < 0 then
        startTime = Time.time
        -- Time.dropped counts from the start of the fight, not the start of the wave.
        startDrop = Time.dropped
    end

    updates = updates + 1

    -- Keep spawning until the full count is out. Laid out in a grid fifty wide across the
    -- 500x300 arena this turn uses, so they are spread rather than stacked and the renderer
    -- has to draw all of them.
    if spawned < BULLETS then
        for _ = 1, PER_STEP do
            if spawned >= BULLETS then break end
            spawned = spawned + 1
            local col = (spawned - 1) % 50
            local row = math.floor((spawned - 1) / 50)
            local b = CreateProjectile("px", col * 10 - 245, row * 10 - 145)
            b.sprite.Scale(5, 5)
            bullets[spawned] = b
        end
    end

    -- Only counted once every bullet is out, so the figure describes the sustained load
    -- rather than the worst frame of the ramp.
    if spawned >= BULLETS and Time.mult > worstMult then
        worstMult = Time.mult
    end

    -- Every bullet moves every step, which is the work the ceiling is deciding whether to
    -- run or skip.
    for i = 1, #bullets do
        local b = bullets[i]
        if b.isactive then
            b.Move(math.sin((updates + i * 7) / 24) * 2.0,
                   math.cos((updates + i * 5) / 31) * 1.5)
        end
    end

    if updates >= TARGET then
        local wall = Time.time - startTime
        local game = updates / 60
        SetGlobal("stress_report_" .. SLOT,
            string.format("t%d %db g%.1f w%.1f %.0ffps d%d",
                          SLOT, BULLETS, game, wall, 60 / math.max(worstMult, 0.0001),
                          Time.dropped - startDrop))
        EndWave()
    end
end

function OnHit(bullet)
    -- Damage is not what this measures. Bullets pass straight through.
end
