//! MameyForge - Smart contract development framework for MameyFutureNode
//!
//! A Hardhat-like CLI tool for building, testing, and deploying WASM smart contracts.

use clap::{Parser, Subcommand};
use console::style;
use tracing::info;

mod commands;
mod config;
mod devnet;
mod artifacts;
mod error;

use commands::{
    init::InitCommand,
    build::BuildCommand,
    test::TestCommand,
    deploy::DeployCommand,
    call::CallCommand,
    query::QueryCommand,
    clean::CleanCommand,
    info::InfoCommand,
};
use devnet::DevnetCommand;
use error::ForgeResult;

/// MameyForge - Smart contract development framework
#[derive(Parser)]
#[command(name = "mameyforge")]
#[command(author = "Mamey Technologies <info@mamey.io>")]
#[command(version)]
#[command(about = "Smart contract development framework for MameyFutureNode", long_about = None)]
#[command(propagate_version = true)]
struct Cli {
    /// Enable verbose output
    #[arg(short, long, global = true)]
    verbose: bool,

    /// Configuration file path
    #[arg(short, long, global = true, default_value = "mameyforge.toml")]
    config: String,

    #[command(subcommand)]
    command: Commands,
}

#[derive(Subcommand)]
enum Commands {
    /// Initialize a new contract project
    Init(InitCommand),

    /// Build contracts to WASM
    Build(BuildCommand),

    /// Run tests
    Test(TestCommand),

    /// Deploy a contract
    Deploy(DeployCommand),

    /// Call a contract function (state-changing)
    Call(CallCommand),

    /// Query a contract function (read-only)
    Query(QueryCommand),

    /// Get contract information
    Info(InfoCommand),

    /// Clean build artifacts
    Clean(CleanCommand),

    /// Devnet management commands
    Devnet(DevnetCommand),
}

#[tokio::main]
async fn main() -> ForgeResult<()> {
    let cli = Cli::parse();

    // Initialize logging
    let filter = if cli.verbose { "debug" } else { "info" };
    tracing_subscriber::fmt()
        .with_env_filter(filter)
        .with_target(false)
        .init();

    // Print banner
    print_banner();

    // Execute command
    match cli.command {
        Commands::Init(cmd) => cmd.execute().await,
        Commands::Build(cmd) => cmd.execute(&cli.config).await,
        Commands::Test(cmd) => cmd.execute(&cli.config).await,
        Commands::Deploy(cmd) => cmd.execute(&cli.config).await,
        Commands::Call(cmd) => cmd.execute(&cli.config).await,
        Commands::Query(cmd) => cmd.execute(&cli.config).await,
        Commands::Info(cmd) => cmd.execute(&cli.config).await,
        Commands::Clean(cmd) => cmd.execute(&cli.config).await,
        Commands::Devnet(cmd) => cmd.execute(&cli.config).await,
    }
}

fn print_banner() {
    println!();
    println!("{}", style("  ╔╦╗┌─┐┌┬┐┌─┐┬ ┬╔═╗┌─┐┬─┐┌─┐┌─┐").cyan());
    println!("{}", style("  ║║║├─┤│││├┤ └┬┘╠╣ │ │├┬┘│ ┬├┤ ").cyan());
    println!("{}", style("  ╩ ╩┴ ┴┴ ┴└─┘ ┴ ╚  └─┘┴└─└─┘└─┘").cyan());
    println!("{}", style("  Smart Contract Development Framework").dim());
    println!();
}
