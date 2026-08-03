-- The boss registry.
--
-- Every entry here is one row in the boss select screen, shown in the order listed.
--
--   id        the encounter script under Lua/Encounters, without the .lua
--   name      the boss name, shown large
--   subtitle  one line shown under the name
--   teased    true for a boss the game advertises rather than ships. It shows on the select
--             screen, can never be picked, and needs no encounter script
--
-- The loader ignores keys it does not recognise, so fields like a portrait or a
-- music track can be added here before the menu is ready to read them.

return {
    { id = "illia",             name = "Illia, The Tutor",   subtitle = "Not built yet" },

    -- Advertised rather than playable. It has no encounter script and no records, because it
    -- can never be started, and its logo is the whole tease, so the name stays hidden. It sits
    -- next to the tutorial on purpose: the first thing the player can do beside the thing they
    -- cannot.
    { id = "teased",            name = "???",                teased = true },
}
