# Accessibility — Ierahkwa Sovereign Digital Platform

## Our Commitment

The Ierahkwa Platform is committed to digital accessibility for all people, including those with disabilities. As a sovereign platform serving 72 million indigenous people worldwide, we recognize that accessibility is a fundamental right, not a feature.

We align with the [GAAD Pledge](https://gaad.foundation/pledge/) and commit to:
1. Empowering people with disabilities to contribute to our open-source ecosystem
2. Ensuring all 49 sovereign platforms meet WCAG 2.2 AA standards
3. Supporting assistive technologies across all interfaces

## Standards

We follow:
- **WCAG 2.2** Level AA compliance
- **WAI-ARIA 1.2** for dynamic content
- **Section 508** of the Rehabilitation Act
- **European Accessibility Act (EAA)** compliance
- **EN 301 549** European standard for ICT accessibility

## Platform Accessibility Features

### Visual
- Minimum 4.5:1 contrast ratio for normal text
- Minimum 3:1 contrast ratio for large text and UI components
- No information conveyed by color alone
- Resizable text up to 200% without loss of functionality
- Dark theme designed with accessibility in mind

### Motor
- Full keyboard navigation across all platforms
- No keyboard traps
- Skip navigation links on all pages
- Focus indicators visible on all interactive elements
- Touch targets minimum 44x44 CSS pixels

### Auditory
- Captions and transcripts for all multimedia content
- No auto-playing audio
- Visual alternatives for all audio cues

### Cognitive
- Clear, consistent navigation patterns
- Plain language content (reading level appropriate for audience)
- Error prevention and recovery guidance
- Consistent identification of UI components

## Assistive Technology Support

Tested with:
- **Screen Readers**: NVDA, JAWS, VoiceOver (macOS/iOS), TalkBack (Android)
- **Voice Control**: Dragon NaturallySpeaking, Voice Control (macOS)
- **Switch Access**: iOS Switch Control, Android Switch Access
- **Screen Magnification**: ZoomText, macOS Zoom
- **Eye Tracking**: Tobii Eye Tracker compatibility

## Indigenous Language Accessibility

- Support for 37+ indigenous languages with proper Unicode rendering
- Right-to-left (RTL) text support where applicable
- Proper language tags (`lang` attribute) for assistive technology
- Cultural content presented with appropriate context

## Sovereign Platform Compliance Matrix

| Platform | WCAG 2.2 AA | Keyboard Nav | Screen Reader | Color Contrast | Status |
|----------|-------------|--------------|---------------|----------------|--------|
| Portal Soberano | ✅ | ✅ | ✅ | ✅ | Compliant |
| Cloud Soberana | ✅ | ✅ | ✅ | ✅ | Compliant |
| Correo Soberano | ✅ | ✅ | ✅ | ✅ | Compliant |
| Docs Soberanos | ✅ | ✅ | ✅ | ✅ | Compliant |
| Code Soberano | ✅ | ✅ | ✅ | ✅ | Compliant |
| Búsqueda Soberana | ✅ | ✅ | ✅ | ✅ | Compliant |
| Canal Soberano | ✅ | ✅ | ✅ | ✅ | Compliant |
| Música Soberana | ✅ | ✅ | ✅ | ✅ | Compliant |
| Voz Soberana | ✅ | ✅ | ✅ | ✅ | Compliant |
| Media Soberana | ✅ | ✅ | ✅ | ✅ | Compliant |
| (All 49 platforms) | ✅ | ✅ | ✅ | ✅ | Target |

## Development Guidelines

### For Contributors

1. **Semantic HTML**: Use proper heading hierarchy, landmarks, lists, and buttons
2. **ARIA**: Add ARIA labels, roles, and states only when native HTML is insufficient
3. **Keyboard**: Ensure all interactive elements are focusable and operable
4. **Color**: Never use color as the sole indicator of state or information
5. **Images**: Provide descriptive alt text for all meaningful images
6. **Forms**: Associate labels with inputs, provide error messages
7. **Motion**: Respect `prefers-reduced-motion` media query
8. **Focus**: Maintain visible focus indicators, manage focus on dynamic content

### Testing Checklist

Before submitting a PR:
- [ ] Keyboard navigation works for all new interactive elements
- [ ] Screen reader announces content correctly
- [ ] Color contrast meets WCAG 2.2 AA minimums
- [ ] No new ARIA errors (test with axe-core)
- [ ] Focus management is correct for dynamic content
- [ ] Text is resizable to 200% without content overlap
- [ ] Touch targets are at least 44x44px
- [ ] Alternative text provided for non-text content

## Reporting Accessibility Issues

If you encounter an accessibility barrier:

1. **GitHub Issues**: Open an issue with the `accessibility` label
2. **Email**: accessibility@ierahkwa.sovereign
3. **Response Time**: We aim to respond within 48 hours

Please include:
- The platform/page where you encountered the barrier
- The assistive technology you were using (if applicable)
- A description of what you were trying to do
- What happened vs. what you expected

## Continuous Accessibility

We use GitHub Agentic Workflows to continuously monitor accessibility:
- Automated WCAG compliance checking on every PR
- Weekly accessibility audit reports
- Continuous monitoring for color contrast regressions
- Automated alt text verification

## References

- [Web Content Accessibility Guidelines (WCAG) 2.2](https://www.w3.org/TR/WCAG22/)
- [WAI-ARIA 1.2](https://www.w3.org/TR/wai-aria-1.2/)
- [GAAD Foundation](https://gaad.foundation/)
- [GAAD Pledge](https://gaad.foundation/pledge/)
- [European Accessibility Act](https://ec.europa.eu/social/main.jsp?catId=1202)

---

*This accessibility statement was last updated on February 22, 2026.*
*Ierahkwa Platform — Sovereign technology for all people, without barriers.*
