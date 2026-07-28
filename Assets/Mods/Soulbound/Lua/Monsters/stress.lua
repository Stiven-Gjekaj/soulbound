-- The monster for the stress encounter. It is a measuring instrument, not a boss.
-- It cannot be killed or spared, so the encounter runs its three turns and the numbers
-- can be read off the screen.

comments = { "It is counting." }
commands = { "Check" }
randomdialogue = { "..." }

sprite = "empty"
name = "Stress Test"
hp = 999999
atk = 1
def = 0
check = "Runs three turns of identical length and reports what they cost."
dialogbubble = "right"

xp = 0
gold = 0

canspare = false
cancheck = true

currentdialogue = { "Turn one.[w:10] Light." }

local said = 0

function HandleAttack(attackstatus)
    -- Unkillable on purpose. Press Escape to leave once the numbers are on screen.
end

-- Called by the encounter script rather than by the engine.
--
-- This was an EnemyDialogueStarting handler and it never ran, back when the engine stopped
-- dispatching a game event as soon as the encounter script handled it. Both scripts define
-- that event, so the encounter's won and this one was dead code. The engine calls both now,
-- but this stays an ordinary function: the encounter decides which turn it is, and the line
-- follows from that rather than from two handlers agreeing about a counter.
function NextLine()
    said = said + 1
    if said == 1 then
        currentdialogue = { "Same length,[w:5] more bullets." }
    elseif said == 2 then
        currentdialogue = { "Same again,[w:5] bigger arena." }
    elseif said == 3 then
        currentdialogue = { "Now the absurd one.[w:10]\nThis will hurt." }
    else
        currentdialogue = { "Read the numbers.[w:10]\nEscape to leave." }
    end
end

function HandleCustomCommand(command)
end
