# Simple Key/Value Message

title = L20n demo
hello = Hello, World!

# An example of a Message using variants (member-lists)
# and references (identifiers)

l20n = L20n
intro = { build ->
    [dev]       You're using a dev build of { l20n }.
    [prod]      You're using a production-ready single-file version of { l20n }.
    *[unknown]  You're using an unknown version of { l20n }.
}

# Text can also be spanned across multiple lines
# as a block-text placeable

screenWidth =
    | The label of the button below changes depending on the
    | available screen width, allowing for a responsive design.

# an example of a brand
[[ brand ]]

# An example showcasing variants used as attributes

brandName = Firefox
  [gender] masculine

opened-new-window = { brandName[gender] ->
 *[masculine] { brandName } otworzyl nowe okno.
  [feminine] { brandName } otworzyla nowe okno.
}

# An example showcasing the use of a builtin

last-notice = Last checked: { DATETIME($lastChecked, day = "numeric", month = "long") }.

# A complex example

liked-photo = { LEN($people) ->
    [1]     { $people } likes
    [2]     { $people } like
    [3]     { TAKE(2, $people), "one more person" } like

   *[other] { TAKE(2, $people),
              "{ LEN(DROP(2, $people)) ->
                  [1]    one more person like
                 *[other]  { LEN(DROP(2, $people)) } more people like
               }"
            }
} your photo.

foo = bar
