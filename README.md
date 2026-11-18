# Template project for Worker Service

[Template](https://usausa.github.io/template/) project for Worker Service.

# ✅ Implement

| Category | Feature |
| --- | --- |
| Job | Schedule mode (named cron schedules in local time, misfire policy, Mofucat.JobScheduler) / Batch mode (command line, exit code) / Named actions / Graceful stop |
| Action | Hello / Cleanup (retention policies by age, extension, size and depth, file and time budget) |
| Result | Summary log (succeeded, skipped, failed, freed, elapsed) / Comparison with the previous run |
| Basic | Setting (validated on start) / Logging (Serilog) / Startup log |
| Operation | Windows service / systemd |

# 📚 Docs

The documents in `docs` are in Japanese.

| Document | Contents |
| --- | --- |
| [settings.md](docs/settings.md) | Schedules, retention policies, results, exit codes, and validation |

# 🔗 Related Projects

| Project | Description |
| --- | --- |
| [template-command-server](https://github.com/usausa/template-command-server) | TCP command server template |
| [template-command-tool](https://github.com/usausa/template-command-tool) | CLI tool template |

# 📦 My Libraries

| Library | Description |
| --- | --- |
| [Usa.Smart.Mapper](https://github.com/usausa/Smart-Net-Mapper) | Object mapper |
| [Mofucat.JobScheduler](https://github.com/usausa/mofucat-job-scheduler) | Job scheduler |
