# WYRD Search - Puzzle Game

**WYRD SEARCH** is an interactive word puzzle game developed using **.NET MAUI**. The player is presented with a grid of letters and must select tiles to form hidden words. Correct guesses are visually confirmed with color changes, and shared letters used in multiple words are uniquely highlighted.

This game is designed to be simple, fun, and a great demonstration of multi-platform app development with .NET MAUI.

---

## Features

This game includes the following features:

- **Grid-Based Gameplay**: A grid of letter buttons appears on the screen. Players tap the tiles in sequence to form words.
- **Word Validation**: Words are checked against a predefined list. If a correct word is found, the buttons turn **gray**.
- **Shared Letters Highlighted**: If a letter belongs to multiple words that haven’t all been found yet, it remains **orange**, making overlapping words clearer.
- **Progress Tracking**: The UI displays how many words the player has found out of the total word list.
- **New Game Button**: Players can restart the game at any time with a clean board and a new word set.

---

## Tech Stack

This project is built using the following tools and technologies:

- **.NET MAUI** – A cross-platform framework for creating native mobile and desktop apps with C# and XAML.
- **C#** – For business logic, event handling, and UI interaction.
- **XAML** – For user interface layout and binding.
- **Visual Studio 2022+** – IDE used for development and debugging.

---

## Getting Started

These instructions will help you set up the project on your local machine for development and testing purposes.

### Prerequisites

Ensure you have the following installed:

- Visual Studio 2022 (or later)
- .NET 7 SDK (or later)
- .NET MAUI workload
