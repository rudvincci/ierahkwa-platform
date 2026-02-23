using System;
using System.Collections.Generic;

namespace TradeX.Core.Models;

/// <summary>
/// Ierahkwa TradeX Exchange - User Model
/// Sovereign Government Trading Platform
/// </summary>
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Ierahkwa Citizenship
    public string? CitizenshipId { get; set; }
    public CitizenshipLevel CitizenshipLevel { get; set; } = CitizenshipLevel.None;
    
    // Verification
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public bool KycVerified { get; set; }
    public KycStatus KycStatus { get; set; } = KycStatus.NotSubmitted;
    
    // Security
    public bool TwoFactorEnabled { get; set; }
    public string? GoogleAuthSecret { get; set; }
    
    // Account Status
    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserRole Role { get; set; } = UserRole.User;
    
    // Referral
    public string? ReferralCode { get; set; }
    public Guid? ReferredBy { get; set; }
    public decimal ReferralEarnings { get; set; }
    
    // VIP - Programa Ierahkwa Sovereign (descuento en fees, prioridad)
    public VipLevel VipLevel { get; set; } = VipLevel.None;
    public decimal VipPoints { get; set; }
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    // Navigation
    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
    public virtual ICollection<Trade> Trades { get; set; } = new List<Trade>();
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Stake> Stakes { get; set; } = new List<Stake>();
}

public enum UserStatus { Pending, Active, Suspended, Banned }
public enum UserRole { User, Vendor, Staff, Admin, SuperAdmin }
public enum KycStatus { NotSubmitted, Pending, Approved, Rejected }
public enum CitizenshipLevel { None, Member, Probation, Resident, Official, Citizen }

/// <summary>Niveles VIP - Descuento en fees y prioridad en transacciones (Ierahkwa Sovereign Exchange)</summary>
public enum VipLevel
{
    None = 0,     // 0 pts
    Bronze = 1,   // 5% descuento
    Silver = 2,   // 10% - Host dedicado
    Gold = 3,     // 15% - Retiros prioritarios
    Platinum = 4, // 20% - Eventos exclusivos
    Diamond = 5,  // 25% - Viajes
    Royal = 6,    // 30% - Todo incluido
    Sovereign = 7 // 50% - Invitación, sin límites
}
