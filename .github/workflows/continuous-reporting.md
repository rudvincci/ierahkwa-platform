---
on:
  schedule: weekly
  permissions:
    contents: read
    issues: read
    pull-requests: read
  safe-outputs:
    create-issue:
      title-prefix: "[weekly-report] "
      labels: [report, continuous-ai]
  tools:
    github:
---

# Weekly Sovereign Platform Report — Ierahkwa Platform

Generate a comprehensive weekly status report for platform maintainers.

## Report sections:

1. **Activity Summary**:
   - New issues opened this week (count and categories)
   - Issues closed this week
   - Pull requests opened, merged, and pending
   - New commits and their areas of impact
   - Contributors active this week

2. **Platform Health**:
   - Total platforms: count directories in `02-plataformas-html/`
   - Total backend services: count services in `03-backend/`
   - CI/CD status: latest workflow run results
   - Test suite health: passing/failing tests
   - Security audit status

3. **Growth Metrics**:
   - New platforms or services added
   - Code changes by directory (frontend vs backend vs infrastructure)
   - Documentation updates
   - Community engagement trends

4. **Ecosystem Progress**:
   - Sovereign platform coverage (which Big Tech services now have alternatives)
   - WAMPUM token economy updates
   - Indigenous language support status
   - Accessibility improvements

5. **Recommendations**:
   - Top 3 priorities for next week
   - Technical debt items to address
   - Community health observations
   - Upcoming milestones or deadlines

## Format:
Use clear markdown with tables, bullet points, and links to relevant issues/PRs. Keep the report actionable and concise. Include a "TL;DR" section at the top with 3-5 key takeaways.
