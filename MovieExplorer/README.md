# MovieExplorer

A .NET MAUI app that demonstrates downloading and displaying a list of movies, local caching, searching, filtering, and saving favourites. It uses .NET 9 and runs on supported MAUI platforms (Android, iOS, macOS, and Windows).

## Features

- Download movie dataset from a public raw JSON file with offline fallback
- Search and filter movies by title and genre
- Sort by title, year, or rating
- Add and remove favourites; favourites are stored locally
- Light/Dark theme toggle persisted using `Preferences`
- MVVM-like ViewModel (`MovieViewModel`) and a `MovieService` for data access

## Requirements

- .NET 9 SDK
- Visual Studio 2022 or later with the .NET MAUI workload installed
- For Android: emulator or device with internet access

## Quick Start

1. Clone the repository:

    ```bash
    git clone https://github.com/g00472270/MovieExplorer.git
    cd MovieExplorer
    ```
2. Open the solution in Visual Studio using __File > Open > Project/Solution__.
3. Select your target platform (Android emulator, iOS simulator, or physical device).
4. Press __F5__ (or use __Debug > Start Debugging__) to build and run the app.

If this resource is unavailable, the app shows a dialog and falls back to a built-in hardcoded dataset.

## Troubleshooting

- If the app shows "Using Offline Movies", check the Debug output for exception details from `MovieService.GetMoviesAsync` (the code logs the exception to `Debug.WriteLine`).
- Verify the raw GitHub URL in `MovieService.cs` and open it in a browser.
- Ensure emulator/device network connectivity and that the Android emulator has internet access.

## Usage

- Browse the movie list on the main page.
- Tap a movie to view its details.
- Add to Favourites using the button on the detail page.
- View saved movies on the Favourites page.
- Remove a movie from favourites using the Delete button.

## AI Acknowledgment

This project was developed with the assistance of AI tools (e.g., Copilot, ChatGPT) for code generation and problem-solving. 

- Copilot: Used for debugging and making the API run correctly
- Claude: Used for assistance in code generation and problem-solving, especially for the UI and ViewModel logic
- ChatGPT: Used to help review README content
