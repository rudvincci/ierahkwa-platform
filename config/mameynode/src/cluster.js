/**
 * MameyNode Cluster Manager — Ierahkwa Platform v3.3.0
 * Multi-process cluster for optimal CPU utilization
 */

import cluster from 'node:cluster';
import { cpus } from 'node:os';
import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';

const CONFIG_PATH = process.env.MAMEYNODE_CONFIG || '/etc/ierahkwa/config.json';
let config;
try {
  config = JSON.parse(readFileSync(resolve(CONFIG_PATH), 'utf-8'));
} catch {
  config = { cluster: { workers: { min: 2, max: 16 } } };
}

const numCPUs = cpus().length;
const minWorkers = config.cluster?.workers?.min || 2;
const maxWorkers = config.cluster?.workers?.max || 16;
const workerCount = Math.min(Math.max(numCPUs, minWorkers), maxWorkers);

if (cluster.isPrimary) {
  console.log(`🌿 MameyNode Cluster v3.3.0`);
  console.log(`   Primary PID: ${process.pid}`);
  console.log(`   CPUs: ${numCPUs} | Workers: ${workerCount}`);
  console.log(`   Launching workers...`);
  console.log('');

  const workers = new Map();

  for (let i = 0; i < workerCount; i++) {
    const worker = cluster.fork();
    workers.set(worker.id, { startedAt: Date.now(), restarts: 0 });
    console.log(`   Worker ${worker.id} (PID: ${worker.process.pid}) started`);
  }

  cluster.on('exit', (worker, code, signal) => {
    const info = workers.get(worker.id) || { restarts: 0 };
    console.warn(`   ⚠ Worker ${worker.id} (PID: ${worker.process.pid}) exited — code: ${code}, signal: ${signal}`);
    workers.delete(worker.id);

    // Restart with backoff
    if (info.restarts < 10) {
      const delay = Math.min(1000 * Math.pow(2, info.restarts), 30000);
      console.log(`   ↻ Restarting worker in ${delay}ms (attempt ${info.restarts + 1})`);
      setTimeout(() => {
        const newWorker = cluster.fork();
        workers.set(newWorker.id, { startedAt: Date.now(), restarts: info.restarts + 1 });
        console.log(`   Worker ${newWorker.id} (PID: ${newWorker.process.pid}) restarted`);
      }, delay);
    } else {
      console.error(`   ✕ Worker exceeded max restarts (10). Not restarting.`);
    }
  });

  // Health monitoring
  setInterval(() => {
    const alive = Object.keys(cluster.workers).length;
    if (alive < minWorkers) {
      console.warn(`   ⚠ Only ${alive}/${workerCount} workers alive. Spawning replacement...`);
      const w = cluster.fork();
      workers.set(w.id, { startedAt: Date.now(), restarts: 0 });
    }
  }, 10000);

  // Graceful shutdown
  const shutdown = (signal) => {
    console.log(`\n   ${signal} — Shutting down cluster...`);
    for (const id in cluster.workers) {
      cluster.workers[id].send('shutdown');
      cluster.workers[id].kill('SIGTERM');
    }
    setTimeout(() => process.exit(0), 10000);
  };

  process.on('SIGTERM', () => shutdown('SIGTERM'));
  process.on('SIGINT', () => shutdown('SIGINT'));

} else {
  // Worker process — start server
  await import('./server.js');
}
