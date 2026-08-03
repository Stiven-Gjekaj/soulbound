-- Illia's encounter.
--
-- The boss is hers; the fight is not built. The monster and the wave it pulls in are both
-- still called placeholder, because that is what they are, and they keep those names until
-- there is something of hers to put in their place.

encountertext = "This fight is not built yet."
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

function HandleItem(ItemID)
    -- Runs when the player uses an item.
end
