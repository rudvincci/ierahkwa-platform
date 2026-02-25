using Microsoft.EntityFrameworkCore;
using Ierahkwa.MessagingService.Domain;

namespace Ierahkwa.MessagingService.Infrastructure;

public class MessagingServiceDbContext : DbContext
{
    public MessagingServiceDbContext(DbContextOptions<MessagingServiceDbContext> options) : base(options) { }

    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Mailbox> Mailboxes => Set<Mailbox>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
}
