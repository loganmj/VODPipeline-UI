# VODPipeline-UI

This is the Blazor Server dashboard for monitoring and managing the VODPipeline system.

This application provides a web frontend for monitoring the video processing pipeline. It interacts with the backend API to display job status, history, and configuration. See the Architecture section below for links to related components.

## Features
- Live job status
- Job history (future)
- Config editor
- Log viewer
- SignalR real-time updates (future)

## Architecture
UI → VODPipeline-API → VODPipeline-DB

### Related Projects
This application is part of a multi-component system:
- **[VODPipeline-API](https://github.com/loganmj/VODPipeline-API)** - Backend API service
- **[VODPipeline-Function](https://github.com/loganmj/VODPipeline-Function)** - Stateless function app for video processing
