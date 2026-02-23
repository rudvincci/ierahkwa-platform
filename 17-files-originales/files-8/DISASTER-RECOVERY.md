# Disaster Recovery Plan — Red Soberana v1.0

## Recovery Objectives
| Metric | Target |
|--------|--------|
| RPO (Recovery Point Objective) | 1 hour |
| RTO (Recovery Time Objective) | 4 hours |
| MTTR (Mean Time to Recover) | 2 hours |

## Tier Classification
| Tier | Systems | RPO | RTO |
|------|---------|-----|-----|
| P0 Critical | MameyNode, BDET Bank, Auth | 15 min | 30 min |
| P1 High | API, AI Fortress, Atabey | 1 hr | 2 hr |
| P2 Medium | All 98 Platforms, CDN | 4 hr | 8 hr |
| P3 Low | Analytics, Logging, Admin | 24 hr | 48 hr |

## Backup Strategy
- **PostgreSQL**: WAL streaming to standby + hourly pg_dump + daily encrypted to S3
- **MameyNode**: 3-node consensus cluster + daily chain snapshot to S3
- **Redis**: RDB snapshots every 15 min + AOF persistence
- **Media (MinIO)**: Cross-region replication to secondary CloudSoberana
- **Secrets (Vault)**: Auto-unseal with KMS + encrypted backup daily

## Failover Procedures
### Database Failure
1. Automated failover via Patroni (PostgreSQL HA)
2. DNS update to standby (automated, <30s)
3. Verify replication lag < 15 min RPO
4. Notify operations team via PagerDuty

### Blockchain Node Failure
1. MameyNode consensus continues with 2/3 nodes
2. Failed node auto-recovers from peers
3. If >1 node fails: restore from latest snapshot
4. Re-sync from genesis if full corruption

### Full Region Failure
1. DNS failover to secondary region (Route53 health checks)
2. Promote read-replicas to primary in secondary region
3. Restore MameyNode from latest snapshot in secondary
4. Verify all services operational within 4hr RTO
5. Post-incident: rebuild primary region, fail back

## Testing Schedule
| Test | Frequency | Last Tested |
|------|-----------|-------------|
| Backup restoration | Monthly | — |
| Database failover | Quarterly | — |
| Full DR simulation | Annually | — |
| Chaos engineering | Weekly | — |

## Communication Plan
1. Incident detected → Auto-alert to on-call engineer
2. 5 min: Incident commander assigned
3. 15 min: Status page updated (status.soberano.bo)
4. 30 min: Community notification via MensajeSoberano broadcast
5. Resolution: Post-mortem within 48 hours
