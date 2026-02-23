# MameyNode Portals Design System

## Overview

This design system provides comprehensive guidelines for building consistent, accessible, and aesthetically superior user interfaces across all MameyNode portals. All components follow Material Design 3.0 principles.

## Material Design 3.0 Compliance

All portals must strictly adhere to Google's Material Design 3.0 guidelines for:
- Spacing and layout
- Elevation and shadows
- Typography
- Color system
- Component patterns
- Motion and animation

## Color System

### Primary Colors
- **Primary**: Used for main actions, links, and key UI elements
- **Primary Variant**: Lighter shade for hover states
- **On Primary**: Text color on primary backgrounds (white or dark)

### Secondary Colors
- **Secondary**: Used for secondary actions and accents
- **Secondary Variant**: Lighter shade for hover states
- **On Secondary**: Text color on secondary backgrounds

### Surface Colors
- **Surface**: Default background color
- **Surface Variant**: Slightly different shade for cards and elevated surfaces
- **On Surface**: Default text color
- **On Surface Variant**: Text color on variant surfaces

### Semantic Colors
- **Success**: Green (#4CAF50) - For success messages and positive actions
- **Warning**: Amber (#FF9800) - For warnings and cautionary messages
- **Error**: Red (#F44336) - For errors and destructive actions
- **Info**: Blue (#2196F3) - For informational messages

### Color Contrast Requirements
- **Normal Text**: Minimum 4.5:1 contrast ratio (WCAG 2.1 AA)
- **Large Text** (18pt+ or 14pt+ bold): Minimum 3:1 contrast ratio
- **Interactive Elements**: Minimum 3:1 contrast ratio

### Color Usage Examples

```razor
<!-- Primary action button -->
<MudButton Variant="Variant.Filled" Color="Color.Primary">
    Submit
</MudButton>

<!-- Success message -->
<MudAlert Severity="Severity.Success">
    Operation completed successfully
</MudAlert>

<!-- Error state -->
<MudAlert Severity="Severity.Error">
    An error occurred
</MudAlert>
```

## Typography

### Type Scale

Material Design 3.0 type scale:

- **H1**: 57px / 64px line height - Page titles
- **H2**: 45px / 52px line height - Section headers
- **H3**: 36px / 44px line height - Subsection headers
- **H4**: 32px / 40px line height - Card titles
- **H5**: 28px / 36px line height - List headers
- **H6**: 24px / 32px line height - Small headers
- **Subtitle1**: 16px / 24px line height - Card subtitles
- **Subtitle2**: 14px / 20px line height - Small subtitles
- **Body1**: 16px / 24px line height - Body text (default)
- **Body2**: 14px / 20px line height - Secondary body text
- **Button**: 14px / 20px line height - Button text
- **Caption**: 12px / 16px line height - Captions and labels
- **Overline**: 12px / 16px line height - Overline text

### Font Family

- **Primary**: Roboto (Material Design default)
- **Monospace**: 'Roboto Mono' or 'Courier New' for code/data

### Typography Usage

```razor
<!-- Page title -->
<MudText Typo="Typo.h1">Dashboard</MudText>

<!-- Section header -->
<MudText Typo="Typo.h3">Recent Transactions</MudText>

<!-- Body text -->
<MudText Typo="Typo.body1">This is body text</MudText>

<!-- Caption -->
<MudText Typo="Typo.caption" Color="Color.Secondary">
    Last updated 2 minutes ago
</MudText>
```

### Responsive Typography

Typography scales appropriately for different screen sizes:
- Mobile: Base size
- Tablet: +2px for headings
- Desktop: +4px for headings

## Spacing System

### Spacing Scale

Material Design uses an 8dp grid system. All spacing values must be multiples of 4dp:

- **4dp**: Tight spacing (between related elements)
- **8dp**: Standard spacing (between elements in a group)
- **16dp**: Comfortable spacing (between groups)
- **24dp**: Generous spacing (between major sections)
- **32dp**: Extra spacing (page margins on mobile)
- **48dp**: Large spacing (page margins on desktop)
- **64dp**: Maximum spacing (between major page sections)

### Container Padding

- **Mobile (xs)**: 16dp horizontal padding
- **Tablet (sm/md)**: 24dp horizontal padding
- **Desktop (lg/xl)**: 32dp horizontal padding

### Card Padding

- **Standard Cards**: 16dp padding
- **Prominent Cards**: 24dp padding
- **Compact Cards**: 8dp padding

### Spacing Usage

```razor
<!-- Use MudBlazor spacing classes -->
<MudStack Spacing="2"> <!-- 16dp spacing -->
    <MudCard>Card 1</MudCard>
    <MudCard>Card 2</MudCard>
</MudStack>

<!-- Or use custom spacing -->
<div style="margin-bottom: 24px;">
    Content with 24dp bottom margin
</div>
```

## Elevation System

### Elevation Levels

Material Design elevation creates depth and hierarchy:

- **0dp**: Flat surfaces (no shadow)
- **1dp**: Hover states, subtle elevation
- **2dp**: Cards, raised buttons
- **4dp**: Dialogs, dropdowns, floating action buttons
- **8dp**: App bars, navigation drawers
- **12dp**: Modal dialogs
- **16dp**: Tooltips, popovers
- **24dp**: Maximum elevation (rarely used)

### Elevation Usage

```razor
<!-- Standard card with 2dp elevation -->
<MudCard Elevation="2">
    Card content
</MudCard>

<!-- Dialog with 12dp elevation -->
<MudDialog Elevation="12">
    Dialog content
</MudDialog>
```

### Shadow System

Material Design shadows follow this pattern:
- **X offset**: 0
- **Y offset**: Elevation value (e.g., 2dp elevation = 2dp Y offset)
- **Blur radius**: Elevation value × 2
- **Spread radius**: 0
- **Color**: Black with opacity based on elevation

## Component Patterns

### Cards

Cards are the primary container for content:

```razor
<MudCard Elevation="2">
    <MudCardContent>
        <MudText Typo="Typo.h5">Card Title</MudText>
        <MudText Typo="Typo.body2" Color="Color.Secondary">
            Card content goes here
        </MudText>
    </MudCardContent>
    <MudCardActions>
        <MudButton>Action</MudButton>
    </MudCardActions>
</MudCard>
```

### Buttons

Button hierarchy:
- **Filled**: Primary actions
- **Outlined**: Secondary actions
- **Text**: Tertiary actions or less important actions

```razor
<!-- Primary action -->
<MudButton Variant="Variant.Filled" Color="Color.Primary">
    Primary Action
</MudButton>

<!-- Secondary action -->
<MudButton Variant="Variant.Outlined" Color="Color.Primary">
    Secondary Action
</MudButton>

<!-- Tertiary action -->
<MudButton Variant="Variant.Text" Color="Color.Primary">
    Tertiary Action
</MudButton>
```

### Forms

Forms follow Material Design patterns:

```razor
<MudForm>
    <MudTextField 
        Label="Email" 
        Variant="Variant.Outlined"
        Required="true"
        RequiredError="Email is required" />
    
    <MudTextField 
        Label="Password" 
        Variant="Variant.Outlined"
        InputType="InputType.Password"
        Required="true" />
    
    <MudButton 
        Variant="Variant.Filled" 
        Color="Color.Primary"
        ButtonType="ButtonType.Submit">
        Submit
    </MudButton>
</MudForm>
```

### Data Tables

Tables with Material Design styling:

```razor
<MudTable Items="@items" Hover="true" Striped="true">
    <HeaderContent>
        <MudTh>Column 1</MudTh>
        <MudTh>Column 2</MudTh>
    </HeaderContent>
    <RowTemplate>
        <MudTd DataLabel="Column 1">@context.Property1</MudTd>
        <MudTd DataLabel="Column 2">@context.Property2</MudTd>
    </RowTemplate>
</MudTable>
```

## Responsive Design

### Breakpoints

- **xs**: 0px - Mobile phones (portrait)
- **sm**: 600px - Mobile phones (landscape), small tablets
- **md**: 960px - Tablets
- **lg**: 1280px - Desktops
- **xl**: 1920px - Large desktops

### Mobile-First Approach

Design for mobile first, then enhance for larger screens:

```razor
<MudGrid>
    <MudItem xs="12" sm="6" md="4" lg="3">
        <!-- Full width on mobile, half on small, quarter on medium, third on large -->
    </MudItem>
</MudGrid>
```

### Navigation Patterns

- **Mobile**: Collapsible drawer navigation
- **Tablet**: Persistent drawer (can be collapsed)
- **Desktop**: Persistent drawer or top navigation

## Accessibility Guidelines

### Keyboard Navigation

- All interactive elements must be keyboard accessible
- Logical tab order
- Clear focus indicators (2dp outline, primary color)
- Skip links for main content

### Screen Readers

- Proper ARIA labels for all interactive elements
- ARIA roles for complex components
- ARIA live regions for dynamic content
- Alt text for images

### Color and Contrast

- Never rely solely on color to convey information
- Use icons, text, or patterns in addition to color
- Maintain WCAG 2.1 AA contrast ratios

### Touch Targets

- Minimum 48x48dp for all touch targets
- Adequate spacing between touch targets (8dp minimum)

## Animation and Motion

### Duration

- **Short**: 150ms - Micro-interactions
- **Medium**: 250ms - Standard transitions
- **Long**: 350ms - Complex animations

### Easing

- **Standard**: Ease-in-out for most transitions
- **Deceleration**: Ease-out for entering elements
- **Acceleration**: Ease-in for exiting elements

### Motion Principles

- Motion should be purposeful and meaningful
- Avoid excessive animation
- Respect user preferences (prefers-reduced-motion)

## Component Usage Guidelines

### When to Use Shared Components

1. **Always prefer** shared components from `MameyNode.Portals.Shared`
2. **Extend** shared components when customization is needed
3. **Create new** shared components for patterns used in 3+ portals
4. **Portal-specific** components only when no shared component fits

### Component Composition

Build complex UIs by composing simple components:

```razor
<PageContainer>
    <SectionHeader Title="Dashboard" Icon="fas fa-chart-line" />
    
    <MudGrid Spacing="3">
        <MudItem xs="12" md="6" lg="4">
            <StatCard 
                Title="Total Revenue"
                Value="$125,000"
                Trend="+12%"
                Icon="fas fa-dollar-sign" />
        </MudItem>
    </MudGrid>
</PageContainer>
```

## Best Practices

### Do's

✅ Use Material Design components consistently
✅ Follow the spacing scale
✅ Maintain proper elevation hierarchy
✅ Ensure WCAG 2.1 AA compliance
✅ Test on multiple screen sizes
✅ Use semantic HTML
✅ Provide loading states
✅ Show error messages clearly
✅ Use icons to enhance meaning
✅ Maintain consistent button styles

### Don'ts

❌ Mix design systems
❌ Use arbitrary spacing values
❌ Create custom components when shared ones exist
❌ Ignore accessibility requirements
❌ Use color alone to convey information
❌ Create inconsistent button styles
❌ Skip loading states
❌ Hide error messages
❌ Use low contrast colors
❌ Ignore responsive breakpoints

## Design Tokens

### Spacing Tokens

```css
--spacing-xs: 4px;
--spacing-sm: 8px;
--spacing-md: 16px;
--spacing-lg: 24px;
--spacing-xl: 32px;
--spacing-2xl: 48px;
--spacing-3xl: 64px;
```

### Elevation Tokens

```css
--elevation-0: 0px;
--elevation-1: 1px;
--elevation-2: 2px;
--elevation-4: 4px;
--elevation-8: 8px;
--elevation-12: 12px;
--elevation-16: 16px;
--elevation-24: 24px;
```

### Color Tokens

Use MudBlazor's built-in color system or define custom tokens:

```css
--color-primary: var(--mud-palette-primary);
--color-secondary: var(--mud-palette-secondary);
--color-success: var(--mud-palette-success);
--color-warning: var(--mud-palette-warning);
--color-error: var(--mud-palette-error);
--color-info: var(--mud-palette-info);
```

## Resources

- [Material Design 3.0 Guidelines](https://m3.material.io/)
- [MudBlazor Documentation](https://mudblazor.com/)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [Material Design Color Tool](https://material.io/resources/color/)

## Component Showcase

See the component showcase page in the application for live examples of all shared components with code snippets and usage guidelines.


