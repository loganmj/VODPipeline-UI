# VODPipeline-UI

This is the Blazor Server dashboard for monitoring and managing the VODPipeline system.

This application is a web frontend to provide monitoring for the stateless function app defined here: https://github.com/loganmj/VODPipeline-Function.

It will do this by interacting with the API defined here: https://github.com/loganmj/VODPipeline-API

## Features
- Live job status
- Job history (future)
- Config editor
- Log viewer
- SignalR real-time updates (future)

## Architecture
UI → VODPipeline-API → VODPipeline-DB
