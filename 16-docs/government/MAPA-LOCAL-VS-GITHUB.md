# Mapa: Local vs GitHub (Mamey-io)

**Fecha:** 21 Enero 2026  
**Nota:** Proyectos separados - NO se fusionan

---

## Resumen

| Ubicación | Cantidad | Estado |
|-----------|----------|--------|
| **LOCAL** (esta computadora) | 35+ proyectos | Independiente |
| **GITHUB** (Mamey-io) | 33 repos | Independiente |

---

## Duplicados Funcionales (Referencia)

Estos proyectos hacen cosas similares pero son **independientes**:

| Función | LOCAL | GITHUB (Mamey-io) |
|---------|-------|-------------------|
| Notificaciones | `NotifyHub/` | `Mamey.FWID.Notifications` |
| Identidad/Ciudadanos | `CitizenCRM/` | `Mamey.Government.Identity` + `Mamey.FWID.Identities` |
| Presupuesto | `BudgetControl/` | `Mamey.SICB.TreasuryDisbursements` |
| Reportes | `ReportEngine/` | `Mamey.SICB.TreatyCompliantBudgetReports` |
| Auditoría | `AuditTrail/` | `Mamey.SICB.TreatyValidators` + `WhistleblowerReports` |
| Templates | `FormBuilder/` | `Mamey.TemplateEngine` |
| Workflows | `DocumentFlow/` | `Mamey.FWID.Sagas` + `FWID.Operations` |
| Activos | `AssetTracker/` + `DigitalVault/` | `Mamey.SICB.TreatyBackedAssetTokenizations` |
| Crypto | `services/rust/` | `Mamey.SICB.ZeroKnowledgeProofs` + `MameyLockSlot` |

---

## Solo en LOCAL (No existe en GitHub)

| Proyecto | Descripción |
|----------|-------------|
| `SmartSchool/` | Sistema educativo |
| `HRM/` | Recursos humanos |
| `TradeX/` | Exchange trading |
| `IDOFactory/` | Launchpad IDO |
| `NET10/` | DeFi (Swap/Farm/Pool) |
| `SpikeOffice/` | Suite oficina |
| `ierahkwa-shop/` | E-commerce |
| `pos-system/` | Punto de venta |
| `inventory-system/` | Sistema inventario |
| `InventoryManager/` | Gestión inventario |
| `ServiceDesk/` | Help desk |
| `MeetingHub/` | Reuniones |
| `AdvocateOffice/` | Sistema legal |
| `ContractManager/` | Contratos |
| `ProcurementHub/` | Adquisiciones |
| `DataHub/` | Hub de datos |
| `RnBCal/` | Calendario |
| `AppBuilder/` | Constructor apps |
| `FarmFactory/` | Yield farming |
| `IerahkwaBankPlatform/` | Plataforma banco |
| `platform/` | UI (50+ HTML) |
| `mobile-app/` | App React Native |
| `OutlookExtractor/` | Extractor emails |

---

## Solo en GITHUB (No existe en Local)

| Repo | Descripción |
|------|-------------|
| `MameyNode` | Core node (Rust) |
| `Mamey` | Base Rust |
| `MameyNode.TypeScript` | SDK TypeScript |
| `MameyNode.Python` | SDK Python |
| `MameyNode.JavaScript` | SDK JavaScript |
| `MameyNode.Go` | SDK Go |
| `MameyFramework` | Framework C# |
| `MameyStack` | Stack config |
| `MameyMemory` | Memory layer |
| `MameyLockSlot` | Distributed locking |
| `Maestro` | AI orchestration |
| `Mamey.SICB.TreasuryKeyCustodies` | Custodia de llaves |
| `Mamey.SICB.TreasuryScorecardAggregators` | Scorecards |
| `Mamey.SICB.TreasuryGovernanceAIAdvisors` | AI Governance |
| `Mamey.SICB.TreasuryExceptions` | Excepciones |
| `Mamey.SICB.TreasuryOverrides` | Overrides |
| `Mamey.SICB.TreasuryIssuances` | Emisión moneda |
| `Mamey.SICB.TreatyCollateralValidationOracles` | Oráculos |
| `Mamey.SICB.TreatyIndexedInflationActuators` | Control inflación |
| `Mamey.SICB.ZeroKnowledgeProofs` | ZKP |

---

## Conteo Final

```
LOCAL:  35+ proyectos C#/.NET + JS/TS + Rust
GITHUB: 33 repos privados + 1 público (Maestro)

DUPLICADOS FUNCIONALES: 9 áreas
ÚNICOS LOCAL: 23+ proyectos
ÚNICOS GITHUB: 20+ repos
```

---

**Nota:** Este documento es solo referencia. Los proyectos se mantienen separados.
