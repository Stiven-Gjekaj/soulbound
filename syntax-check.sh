#!/bin/bash
# Syntax-checks every engine script with the Mono compiler.
#
# This is not a build. UnityEngine and MoonSharp are not on the path here, so every file
# raises missing-type errors and those are expected. What this catches is the class of
# mistake that needs no Unity to detect and that CI otherwise takes three minutes to
# report: an unbalanced brace, a dangling else, a stray paren, an unterminated string.
#
# It looks for parser and lexer errors only. C# puts those in the CS1xxx range; CS0116 is
# included because "a namespace cannot directly contain members" is what a missing closing
# brace usually looks like from the outside.
#
# Exits non-zero if any is found.
set -u
cd "$(dirname "$0")" || exit 2

OUT=$(mcs -target:library -out:/dev/null \
        $(find Assets/Scripts -name '*.cs' | tr '\n' ' ') 2>&1)

SYNTAX=$(printf '%s\n' "$OUT" | grep -E 'error (CS1[0-9]{3}|CS0116)\b')

if [ -n "$SYNTAX" ]; then
    echo "Syntax errors:"
    printf '%s\n' "$SYNTAX"
    exit 1
fi

echo "No syntax errors in $(find Assets/Scripts -name '*.cs' | wc -l) files."
