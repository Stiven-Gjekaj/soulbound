-- A development encounter, not a boss.
--
-- It exists to answer one question: does a fixed amount of fight logic take a fixed amount
-- of game time, whatever the frame rate is doing? Every wave here runs for exactly the same
-- number of Update calls and then ends itself, so its length is a number of battle steps
-- rather than a number of seconds. The waves differ only in how many bullets they push, and
-- each reports the game time and wall clock it actually took.
--
-- If the battle tick is doing its job, every turn reports the same game time no matter how
-- badly the renderer struggled. See docs/project/boss-rush-loop.md, "The battle tick".

encountertext = "Three turns, same length each. Watch the numbers."
nextwaves = { "stress_light" }
wavetimer = 999.0
arenasize = { 155, 130 }

enemies = { "stress" }
enemypositions = { { 0, 0 } }

local turn = 0

function EncounterStarting()
    SetGlobal("stress_report_1", "")
    SetGlobal("stress_report_2", "")
    SetGlobal("stress_report_3", "")
end

-- Picks the next turn's wave and arena. nextwaves is re-read at the start of every
-- defending round, so changing it here is what makes the turns differ.
function EnemyDialogueStarting()
    turn = turn + 1
    if turn > 3 then turn = 3 end

    -- The wave reads this to know which report slot it owns, so three turns give three
    -- numbers instead of overwriting each other.
    SetGlobal("stress_slot", turn)

    if turn == 1 then
        nextwaves = { "stress_light" }
        arenasize = { 155, 130 }
    elseif turn == 2 then
        nextwaves = { "stress_heavy" }
        arenasize = { 250, 180 }
    else
        -- Same wave as turn 2 in a much larger arena, so the renderer has more to draw for
        -- identical fight logic. Game time should not notice.
        nextwaves = { "stress_heavy" }
        arenasize = { 400, 250 }
    end
end

-- Collects whatever the waves reported and puts it where the player can read it.
function DefenseEnding()
    local lines = {}
    for i = 1, 3 do
        local report = GetGlobal("stress_report_" .. i)
        if report ~= nil and report ~= "" then
            table.insert(lines, report)
        end
    end
    if #lines > 0 then
        encountertext = table.concat(lines, "\n")
    end
end

function EnemyDialogueEnding()
end

function HandleItem(ItemID)
end
