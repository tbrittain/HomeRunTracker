# HomeRunTracker

HomeRunTracker is an application that tracks MLB home runs in near real-time and displays them on a dashboard. It does this via a backend service that polls the MLB API and notifies connected clients (the dashboard) via a SignalR connection.

## Project Structure

The solution is split into three projects:

- `HomeRunTracker.Backend`: The backend service that polls the MLB API and notifies connected clients via SignalR.
- `HomeRunTracker.Frontend`: The dashboard that displays the home runs as they are hit.
- `HomeRunTracker.Common`: Common code shared between the backend and frontend.

### Backend

The backend service is a .NET Core API composed of some minimal APIs, a SignalR hub, an Orleans silo, and background service that polls the MLB Schedule API daily for game information. Upon new games being found, the background service spawns a new Orleans grain that is resposible for polling live updates from the game upon the game starting. The Orleans grain then notifies the SignalR hub upon new home runs or updates to existing home runs (such as the highlight video being uploaded), which in turn notifies connected clients.

### Frontend

The frontend is a Blazor Server application that connects to the backend service via SignalR and displays the home runs as they are hit. It also queries the backend service home run API for home runs that have already been hit. Users are also able to change the date to view home runs from previous days.

## Why not just poll the API directly without the backend service?

I wanted to experiment with SignalR and Microsoft Orleans, and this seemed like a good use case. Finding streaming data for free is difficult, and converting a polling API to a streaming API is a good way to experiment.

On another note, it might be a good idea to do this to add a stateful layer between the MLB API and the dashboard. This would allow for more complex logic to be added to the backend service, such as filtering out home runs that are not interesting (e.g. home runs that are not walk-offs, home runs that are not hit by a player on your fantasy team, etc.).

Also, this prevents the frontend from having to poll the MLB API directly and instead only receive updates when a home run is hit.