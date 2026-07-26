-- Third placeholder encounter, so the boss select has a list worth paging through.
-- It reuses the placeholder monster and wave, and skips the empty callbacks that
-- placeholder.lua carries as a template. Replace it once real content lands.

encountertext = "The third placeholder. Nothing here either."
nextwaves = { "placeholder" }
wavetimer = 4.0
arenasize = { 155, 130 }

enemies = { "placeholder" }
enemypositions = { { 0, 0 } }

function HandleSpare()
    State("ENEMYDIALOGUE")
end
