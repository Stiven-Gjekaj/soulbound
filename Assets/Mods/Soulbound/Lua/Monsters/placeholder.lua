-- Placeholder monster for the Soulbound mod.
-- It draws an engine sprite from the Default folder so the mod ships no art of its own.
-- Replace it once real content lands.

comments = { "Waiting to be written." }
commands = { "Check" }
randomdialogue = { "..." }

sprite = "empty"
name = "Illia"
hp = 10
atk = 1
def = 0
check = "A monster that does not exist yet."
dialogbubble = "right"

xp = 0
gold = 0

canspare = true
cancheck = true

currentdialogue = { "..." }

function HandleAttack(attackstatus)
    if attackstatus == -1 then
        -- The player pressed Fight but missed.
    else
        -- The player hit the monster.
    end
end

function HandleCustomCommand(command)
    -- Runs for any ACT command not handled by the engine.
end
