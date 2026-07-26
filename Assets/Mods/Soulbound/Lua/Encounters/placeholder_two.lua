-- Second placeholder encounter, so the boss select has more than one row to show.
-- It reuses the placeholder monster and wave, and skips the empty callbacks that
-- placeholder.lua carries as a template. Replace it once real content lands.

encountertext = "The second placeholder. Still nothing here."
nextwaves = { "placeholder" }
wavetimer = 4.0
arenasize = { 155, 130 }

enemies = { "placeholder" }
enemypositions = { { 0, 0 } }

function HandleSpare()
    State("ENEMYDIALOGUE")
end
