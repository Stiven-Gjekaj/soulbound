-- The boss registry.
--
-- Every entry here is one row in the boss select screen, shown in the order listed.
--
--   id        the encounter script under Lua/Encounters, without the .lua
--   name      the boss name, shown large
--   subtitle  one line shown under the name
--
-- The loader ignores keys it does not recognise, so fields like a portrait or a
-- music track can be added here before the menu is ready to read them.

return {
    { id = "placeholder", name = "Placeholder", subtitle = "Not built yet" },
}
