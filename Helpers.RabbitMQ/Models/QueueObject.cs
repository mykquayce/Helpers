namespace Helpers.RabbitMQ.Models;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "3rd party")]
public record QueueObject(bool auto_delete, int consumers, bool durable, int messages, string name, string node, string state, string vhost);
