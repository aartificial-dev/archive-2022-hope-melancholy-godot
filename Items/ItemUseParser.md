# Item Action Language Reference

## Table of contents

| Title | Description |
|:------|:------------|
| [Regions](#regions) | Describes defining action region. |
| [Commands List](#commands-list) | Contains list of available commands to use in item actions language. |
| [Examples](#examples) | Contains several exaples on how to use item action language. |

-------------------------------------------------------------------------------

## Regions
> - [define](#inventory-name-action)
> - [comments](#~-comment)

### `define -action`
**Description:** Defines beginning of code region that will be executed when `action` will be performed with item. Region ends with next `define` command or with end of text/file. Can be used with no particular order of regions.
**Arguments:**
> -action: string
> Action name that describes type of interact with item. Accepts `pickup`, `use`, `drop`.

<br>

### `~ comments`
**Description:** `~` (Tilde) defines line that will be ignored and not processed by language parser. Used for comments.
**Arguments:**
> Accepts any text after `~` (tilde)  symbol. This text will be ignored by parser.

-------------------------------------------------------------------------------

## Commands list
> - [inventory](#inventory-name-action)
> - [message](#message-text)
> - [heal](#heal-attribute-value)
> - [damage](#damage-attribute-value)
> - [note](#note-type)
> - [ammo](#ammo-type-amount)
> - [purge](#purge)


### `inventory -name -action`
**Description:** Enables/Disables inventory with `name`.
**Arguments:**
> -name: string
> Name of inventory to enable/disable.

> -action: string
> Type of action to do with inventory. Accepts `enable`, `disable`, `toggle`.

<br>

### `message -text`
**Description:** Shows a message with `text`;
**Arguments:**
> -text: string
> Text string to show in message.

<br>

### `heal -attribute -value`
**Description:** Adds `value` to player's `attribute`.
**Arguments:**
> -attribute: string
> Player attribute to add value to. Accepts `health`, `sanity`.

> -value: integer
> Value to add to attribute.

<br>

### `damage -attribute -value`
**Description:** Subtracts `value` from player's `attribute`.
**Arguments:**
> -attribute: string
> Player attribute to subtract value from. Accepts `health`, `sanity`.

> -value: integer
> Value to subtract from attribute.

<br>

### `note -type`
**Description:** Opens a note of `type` with text defined in item text field.
**Arguments:**
> -type: string
> Type of note to open. Accepts `sticker`, `lined-paper`, `blank-paper`, `newspaper`, `book`.

<br>

### `ammo -type -amount`
**Description:** Adds `amount` to player's ammo `type`.
**Arguments:**
> -type: string
> Ammo type. Accepts `handgun`, `battery`.

> -amount: integer
> Amount of ammo to add.

<br>

### `purge`
**Description:** Deletes item. For better description look at [purge example](#purge-example).
**Arguments:**
> Acepts no arguments

-------------------------------------------------------------------------------

## Examples
> - [Region example](#region-example)
> - [Multiple commands example](#multiple-commands-example)
> - [Comments example](#comments-example)
> - [Ammo pickip](#ammo-pickip)
> - [Purge example](#purge-example)

### Region example

> ~ defining region for pickup action
> define pickup
> message I think this might be useful to combine things
>
> ~ defining region for use action
> define use
> inventory toolbox toggle
>
> ~ defining region for drop action
> define drop
> inventory toolbox disable

This example shows how to use regions to make item that will use `pickup` action to tell what item do, `use` action to toggle toolbox inventory state and `drop` action to disable inventory when item no more present.

### Multiple commands example

> define pickup
> ammo handgun 5
> message There's less then i expected
> 
> define use
> ammo handgun 20
> message Oh, I just didn't notice
> purge

Here's shown possibility of using multiple commands in region. All commands of region will be performed in it's defined action (except commands after [purge](#purge-example) command).

### Comments example

> ~ this line will be ignored by parser
> define use
> ~ this line will be ignored too
>
> ammo ~ and this will produce error because `~` (tilde) not in beginning of line

### Ammo pickip

> define pickup
> ammo handgun 25
> purge

This example shows how to make item perform certain action on pickup and delete itself after.

### Purge example

> define use
> heal health 50
> purge
> ~ next command will be ignored
> heal sanity 50

As example above this example shows how to delete item after performing certain action. `purge` command can be used in any region and no more commands will be executed after it in current region. 