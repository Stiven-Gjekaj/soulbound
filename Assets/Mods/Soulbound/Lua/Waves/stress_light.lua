-- The light turn: few bullets, so the renderer keeps up.
--
-- Runs for exactly TARGET Update calls and then ends itself, so the turn's length is a
-- number of battle steps rather than a number of seconds. It reports the game time that
-- represents alongside the wall clock it actually took, and the worst single frame it saw.
--
-- game and wall agreeing means the fight advanced at its proper rate. game shorter than
-- wall means steps were dropped, which happens when a frame takes long enough to owe more
-- than the tick's catch-up ceiling. Either way the number is the truth about the run.

local BULLETS = 12
local TARGET  = 600         -- ten seconds of game time at sixty steps a second
-- Which turn this is, set by the encounter before the wave is compiled. Floored and
-- defaulted because it reaches Lua as a number and is used as an integer.
local SLOT    = math.floor(tonumber(GetGlobal("stress_slot")) or 1)

local updates   = 0
local startTime = -1
local worstMult = 0
local bullets   = {}

function Update()
    if startTime < 0 then
        startTime = Time.time
        for i = 1, BULLETS do
            local col = (i - 1) % 24
            local row = math.floor((i - 1) / 24)
            local b = CreateProjectile("px", col * 11 - 126, row * 9 - 70)
            b.sprite.Scale(5, 5)
            bullets[i] = b
        end
    end

    updates = updates + 1
    if Time.mult > worstMult then
        worstMult = Time.mult
    end

    -- Every bullet moves every step. Written as movement per Update, which is the thing
    -- that used to change speed with the frame rate.
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
            string.format("turn %d, %d bullets: %.1fs game, %.1fs wall, worst frame %.0f fps",
                          SLOT, BULLETS, game, wall, 60 / math.max(worstMult, 0.0001)))
        EndWave()
    end
end

function OnHit(bullet)
    -- Damage is not what this measures. Bullets pass straight through.
end
