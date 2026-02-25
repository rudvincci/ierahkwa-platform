using Microsoft.EntityFrameworkCore;
using Ierahkwa.LanguageService.Domain;

namespace Ierahkwa.LanguageService.Infrastructure;

public class LanguageServiceDbContext : DbContext
{
    public LanguageServiceDbContext(DbContextOptions<LanguageServiceDbContext> options) : base(options) { }

    public DbSet<TranslationJob> TranslationJobs => Set<TranslationJob>();
    public DbSet<LanguagePair> LanguagePairs => Set<LanguagePair>();
    public DbSet<VoiceCommand> VoiceCommands => Set<VoiceCommand>();
    public DbSet<Glossary> Glossarys => Set<Glossary>();
    public DbSet<PronunciationGuide> PronunciationGuides => Set<PronunciationGuide>();}
