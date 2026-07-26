-- Placeholder encounter for the Soulbound mod.
-- It exists so the engine has one selectable encounter to boot into.
-- Replace it once real content lands.

encountertext = "Soulbound is not built yet."
nextwaves = { "placeholder" }
wavetimer = 4.0
arenasize = { 155, 130 }

enemies = { "placeholder" }
enemypositions = { { 0, 0 } }

function EncounterStarting()
    -- Runs once when the encounter loads.
end

function EnemyDialogueStarting()
    -- Runs before the enemies show their dialogue bubbles.
end

function EnemyDialogueEnding()
    -- Runs after the enemies finish their dialogue bubbles.
end

function DefenseEnding()
    -- Runs when a wave ends.
end

function HandleSpare()
    State("ENEMYDIALOGUE")
end

function HandleItem(ItemID)
    -- Runs when the player uses an item.
end
