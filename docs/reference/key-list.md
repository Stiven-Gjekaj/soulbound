# Key list

`Input.GetKey` uses Unity key codes. A complete list
[can be found here](https://docs.unity3d.com/2018.4/Documentation/ScriptReference/KeyCode.html).

## Regular alphabet

All regular alphabet characters are exactly the same between their key names and the engine Key IDs.
For instance, the "A" key will be `A` in the engine.

Note that using `Input.GetKey("A")`, for instance, will detect the lowercase "a".

## F-keys

All F-keys are exactly the same between their key names and the engine Key IDs. For instance, the
`F12` key will be `F12` in the engine. For reference, the supported F-keys are `F1` through `F15`.

## Keyboard number keys

| Keyboard key | Engine ID |
| --- | --- |
| 1 | Alpha1 |
| 2 | Alpha2 |
| 3 | Alpha3 |
| 4 | Alpha4 |
| 5 | Alpha5 |
| 6 | Alpha6 |
| 7 | Alpha7 |
| 8 | Alpha8 |
| 9 | Alpha9 |
| 0 | Alpha0 |

## Keypad keys

| Keyboard key | Engine ID |
| --- | --- |
| 1 (End) | Keypad1 |
| 2 (Down) | Keypad2 |
| 3 (Page Down) | Keypad3 |
| 4 (Left) | Keypad4 |
| 5 | Keypad5 |
| 6 (Right) | Keypad6 |
| 7 (Home) | Keypad7 |
| 8 (Up) | Keypad8 |
| 9 (Page Up) | Keypad9 |
| 0 (Insert) | Keypad0 |
| . (Del) | KeypadPeriod |
| / | KeypadDivide |
| * | KeypadMultiply |
| - | KeypadMinus |
| + | KeypadPlus |
| Enter | KeypadEnter |
| = | KeypadEquals |

## Keyboard arrow keys

| Keyboard key | Engine ID |
| --- | --- |
| Up | UpArrow |
| Down | DownArrow |
| Right | RightArrow |
| Left | LeftArrow |

## Special keyboard keys

Note: the `%` key cannot be detected as of this moment.

| Keyboard key | Engine ID |
| --- | --- |
| Backspace | Backspace |
| Tab | Tab |
| Return/Enter | Return |
| Pause | Pause |
| Space Bar | Space |
| Escape | Escape |
| `!` | Exclaim |
| `@` | At |
| `#` | Hash |
| `$` | Dollar |
| `^` | Caret |
| `&` | Ampersand |
| `*` | Asterisk |
| `(` | LeftParen |
| `)` | RightParen |
| `-` | Minus |
| `+` | Plus |
| `_` | Underscore |
| `=` | Equals |
| `:` | Colon |
| `;` | Semicolon |
| `"` | DoubleQuote |
| `'` | Quote |
| `,` | Comma |
| `.` | Period |
| `\` | Backslash |
| `/` | Slash |
| `?` | Question |
| `<` | Less |
| `>` | Greater |
| `[` | LeftBracket |
| `]` | RightBracket |
| `` ` `` | BackQuote |

## Misc. keyboard keys

| Keyboard key | Engine ID |
| --- | --- |
| Insert | Insert |
| Home | Home |
| Delete | Delete |
| End | End |
| Page Up | PageUp |
| Page Down | PageDown |
| Num Lock | Numlock |
| Caps Lock | CapsLock |
| Scroll Lock | ScrollLock |
| Right Shift | RightShift |
| Left Shift | LeftShift |
| Right Control | RightControl |
| Left Control | LeftControl |
| Right Alt | RightAlt |
| Left Alt | LeftAlt |
| Right Command | RightCommand |
| Left Command | LeftCommand |
| Right Apple Key | RightApple |
| Left Apple Key | LeftApple |
| Right Windows Key | RightWindows |
| Left Windows Key | LeftWindows |
| Alt Gr | AltGr |
| Print Screen/Sys Rq | SysReq |
| Break | Break |
| Menu | Menu |

## Mouse inputs

| Mouse input | Engine ID |
| --- | --- |
| Left Mouse Button | Mouse0 |
| Right Mouse Button | Mouse1 |
| Scroll Wheel Press | Mouse2 |
| Mouse 3 | Mouse3 |
| Mouse 4 | Mouse4 |
| Mouse 5 | Mouse5 |
| Mouse 6 | Mouse6 |

## New in v0.6.6. Controller buttons

All controller buttons follow the same format: `JoystickXButtonY`

- X represents the number of the controller currently in use, between 1 and 8.
- Y represents the number of the button checked, between 0 and 19.

Note: It is not recommended to directly query a user's controller inputs, as these can
greatly vary between users. It would be best to make a keybind and let the user bind their
controller buttons to it, or to restrict yourself to standard the engine keys.

## New in v0.6.6. Controller axes (joysticks, D-pad, triggers)

Most controller axes follow the same format: `AxisX-Y +` or `AxisX-Y -`

- X represents the number of the axis currently checked, between 3 and 10.
- Y represents the number of the controller currently in use, between 1 and 2. the engine currently
  only handles axes for up to 2 controllers.

Axes 1 and 2 have specific names: Axis 1 is named `HorizontalY`, while Axis 2 is named
`VerticalY`, following the same scheme as above.

Note: It is not recommended to directly query a user's controller inputs except for
`HorizontalY` and `VerticalY`, as these can greatly vary between users. It would be best to
make a keybind and let the user bind their controller buttons to it, or to restrict yourself
to standard the engine keys.
