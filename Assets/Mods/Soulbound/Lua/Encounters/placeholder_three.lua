-- Third placeholder encounter, so the boss select has a list worth paging through.
-- It reuses the placeholder monster and wave, and skips the empty callbacks that
-- placeholder.lua carries as a template. Replace it once real content lands.

-- Broken by hand: the battle text box does not wrap, so a line wider than the box crosses
-- its border and is clipped by the screen.
encountertext = "The third placeholder.\nNothing here either."
nextwaves = { "placeholder" }
wavetimer = 4.0
arenasize = { 155, 130 }

enemies = { "placeholder" }
enemypositions = { { 0, 0 } }
