# Tech Mogul — Complete Game Design Document

**Version:** 0.1 first-pass full GDD  
**Date:** 2026-05-02  
**Project title:** Tech Mogul (working title)  
**Genre:** UI-based sandbox management simulation  
**Target platform:** Windows PC  
**Primary input:** Mouse and keyboard  
**Engine:** TBD  
**Player role:** Founder of a software and hardware startup company  
**Design basis:** Current conversation plus reference research into modern management sims  

---

## Document Control

This is the first complete GDD pass for Tech Mogul. It defines the intended game, systems, player flow, MVP scope, and future expansion space. It is not a final balance document. Numeric values in this document are first-pass prototype values unless marked as locked.

### Design status labels

- **Locked:** Design direction should be treated as committed unless deliberately revised.
- **Prototype value:** A starting value for implementation and testing; expected to change through balance passes.
- **TBD:** Not decided yet; do not treat as final.
- **Deferred:** Intended for later versions, not required for the first playable build.

### Non-infringement rule

Reference games are used for genre analysis and design inspiration only. Tech Mogul must not copy proprietary UI layouts, names, mechanics, exact data, branding, writing, or balancing from any reference title.

---

# GDD_00 — Master Index & Design Authority

## 00.1 Purpose

The GDD defines the design of Tech Mogul: what the game is, what the player does, what systems exist, how systems interact, and what belongs in the first playable version.

The GDD should answer four practical questions:

- What is the fantasy?
- What decisions does the player make?
- What consequences do those decisions create?
- What must the development team build first?

## 00.2 Core project identity

Tech Mogul is a UI-based sandbox management simulation about founding and growing a software and hardware company.

The player acts as the company Founder. The Founder hires employees, forms teams, assigns teams to products and other work, manages finances, studies markets, responds to reports, and builds a technology portfolio over time.

The core fantasy is:

> “I founded a tech company. I choose what we build, who builds it, when we launch it, how we compete, and whether we become a forgotten startup or a market-defining giant.”

## 00.3 Reference influence summary

The main references are:

- **Startup Company:** accessible startup growth, website/product scaling, hosting pressure, marketing, and competing against large tech companies.
- **Software Inc.:** software and hardware company simulation, employee management, teams, support, research, marketing, hardware/software products, and simulated competition.
- **Football Manager 26:** long-term management flow, inbox/portal structure, recruitment hub, calendar-driven progression, data-heavy UI, search/bookmarks, and “continue until something important happens.”
- **Game Dev Tycoon:** approachable product creation, research unlocks, post-product reports, custom engines, and company growth pacing.
- **Mad Games Tycoon 2:** small studio to corporation growth, server rooms, production facilities, console/hardware ambition.
- **Computer Tycoon:** hardware, operating systems, infrastructure, marketing, global competition, research race, and visible competitor products.
- **Capitalism Lab:** market economics, product competition, research, pricing, brand, macro conditions, and replayable business simulation.
- **Big Ambitions:** “from nothing to empire” business growth, staff, logistics, infrastructure, and founder-scale ambition.
- **Motorsport Manager:** long-term planning with high-pressure event resolution; product launches should feel like race-day moments where preparation matters.

## 00.4 Authority order

When design documents conflict, use this order:

1. Current signed-off project requirements from the developer/user.
2. This GDD master section.
3. The relevant numbered GDD section.
4. Implementation plans.
5. Temporary implementation decisions.

If a later implementation plan contradicts this GDD, the implementation plan must either update the GDD or mark the contradiction as temporary.

## 00.5 Master section index

- GDD_00 — Master Index & Design Authority
- GDD_01 — Core Game Overview
- GDD_02 — Core Game Loop & Time Flow
- GDD_03 — Founder, Company Creation & Sandbox Setup
- GDD_04 — Employees & Hiring
- GDD_05 — Teams & Assignments
- GDD_06 — Products Overview
- GDD_07 — Software Products
- GDD_08 — Hardware Products
- GDD_09 — Product Ecosystems & Platform Strategy
- GDD_10 — Market, Competitors & Trends
- GDD_11 — Finance, Revenue & Runway
- GDD_12 — Contracts & Short-Term Work
- GDD_13 — Research & Technology Progression
- GDD_14 — Inbox, Reports & Founder Portal
- GDD_15 — Events, Crises & Launch Moments
- GDD_16 — UI Structure
- GDD_17 — Save/Load & Persistent Simulation
- GDD_18 — First Playable Version Scope
- Appendix A — Core Data Objects
- Appendix B — First-Pass Balance Values
- Appendix C — Example Player Stories
- Appendix D — Glossary
- Appendix E — Source List

## 00.6 Locked high-level direction

The following are locked for the first full design pass:

- The game is a sandbox management sim.
- The player is the Founder of a software and hardware startup.
- The game is UI-based rather than direct-avatar or 3D office-control focused.
- The player hires employees.
- Employees are organised into teams.
- Teams are assigned to products and other company work.
- Products include software, hardware, and later hybrid ecosystem products.
- Time uses a manual Continue flow inspired by long-form management sims.
- Reports, inbox messages, calendars, dashboards, and tables are core gameplay surfaces.
- The first playable build focuses on company management, employees, teams, products, contracts, finance, market simulation, competitors, reports, and save/load.

## 00.7 First-version non-goals

The first playable version should not include:

- Full 3D office construction.
- Physical employee pathfinding.
- Deep furniture placement.
- Stock market ownership systems.
- Mergers and acquisitions.
- Detailed legal/regulatory systems.
- Multiplayer.
- Founder life-sim mechanics.
- Full global logistics simulation.
- Full manufacturing factory simulation.
- Complex modding tools.
- A campaign story mode.

These features can be considered later if the core management loop works.

---

# GDD_01 — Core Game Overview

## 01.1 High concept

Tech Mogul is a UI-driven sandbox management sim where the player builds a technology company from a fragile startup into a serious software, hardware, or platform business.

The game focuses on founder-level decisions: hiring, team structure, product strategy, market timing, finance, infrastructure, support, research, and long-term company direction.

## 01.2 Player fantasy

The player fantasy is not “I personally code every feature.” It is:

- I decide what the company should build.
- I find people who can build it.
- I organise them into teams.
- I commit money, time, and reputation.
- I read signals from the market.
- I launch products and live with the consequences.
- I build a portfolio and ecosystem over multiple years.

## 01.3 Target audience

Primary audience:

- Management sim players.
- Tycoon/business sim players.
- Strategy players who enjoy long-term planning.
- Players who like Football Manager-style menus, calendars, reports, and progression.
- Players interested in startups, software, hardware, or tech-company fantasy.

Secondary audience:

- Game dev/business simulation fans.
- Players who enjoy spreadsheet-adjacent optimisation but still want readable feedback.
- Players who enjoy sandbox play without a fixed campaign objective.

## 01.4 Unique selling proposition

The key differentiator is **Product Ecosystem Management**.

The player does not only produce isolated apps. Products can reinforce or weaken each other. A company can become valuable by building ecosystems:

- Operating system + development tools + marketplace.
- Game console + first-party games + online service.
- Smartphone + mobile OS + app ecosystem.
- SaaS platform + enterprise sales + support infrastructure.
- Hardware device + subscription service + companion app.

This gives the player multiple strategic identities and makes long-term portfolio design more important than simply launching one product after another.

## 01.5 Core design pillars

### Pillar 1 — Founder-level control

The player makes company-level decisions, not minute-by-minute worker inputs.

### Pillar 2 — Teams are the production engine

Employees matter individually, but teams are the main work unit.

### Pillar 3 — Products are living businesses

Launched products continue to generate revenue, support load, bugs, reputation effects, updates, churn, and market share changes.

### Pillar 4 — The market moves without the player

Competitors launch products, trends shift, customer expectations change, talent markets fluctuate, and technology becomes obsolete.

### Pillar 5 — UI is the gameplay space

The game is played through dashboards, reports, comparisons, filters, search, calendar events, inbox items, and data-rich pages.

### Pillar 6 — Preparation creates launch drama

Major launches and crises should feel like “event days” where prior planning matters.

## 01.6 Game modes

### Sandbox Mode — first priority

The main mode. The player configures the starting conditions and plays indefinitely.

### Scenario Mode — deferred

Preset challenges such as “Survive a recession,” “Build a console ecosystem,” or “Compete against an incumbent OS.” Deferred until the sandbox is stable.

### Tutorial Scenario — recommended for later first release

A guided early-game scenario teaching hiring, team creation, product creation, Continue, reports, launch, and finance.

## 01.7 Player goals

Tech Mogul is a sandbox, so the player defines success. The game should still support measurable objectives:

- Reach profitability.
- Launch a successful product.
- Reach a target valuation.
- Dominate a market segment.
- Build a product ecosystem.
- Survive a recession.
- Become the top employer in a region.
- Build a hardware platform with strong software support.
- Reach a cash or revenue milestone.
- Maintain high product reputation over time.

## 01.8 Failure and recovery

The primary failure pressure is insolvency: the company runs out of cash and cannot meet obligations.

Failure should not immediately erase the save unless the player chooses hard failure mode. Recommended options:

- **Standard:** insolvency triggers emergency restructuring options.
- **Hardcore:** bankruptcy ends the save.
- **Sandbox:** negative cash can continue with warnings and reduced options.

Emergency restructuring options are deferred from the first playable build unless simple to implement.

## 01.9 Tone and presentation

The tone should be professional, readable, and data-first.

The game should avoid excessive jokes or parody. Light flavour is acceptable, but the main fantasy is serious company management.

---

# GDD_02 — Core Game Loop & Time Flow

## 02.1 Core loop

The core loop is:

1. Review Founder Portal.
2. Read inbox, reports, calendar events, and company metrics.
3. Decide priorities.
4. Hire employees or restructure teams.
5. Create or revise product plans.
6. Assign teams to work.
7. Press Continue.
8. Time advances.
9. Work progresses, finances update, candidates respond, competitors act, and reports arrive.
10. The player reacts and repeats.

## 02.2 Decision loop

Every major decision should involve three costs:

- **People:** which employees or teams are committed.
- **Time:** how long the work takes and what deadlines are affected.
- **Money:** payroll, infrastructure, production, marketing, and opportunity cost.

Every major decision should produce at least one visible consequence.

## 02.3 Time model

Recommended model:

- The game is paused while the player reviews screens.
- The player presses **Continue** to advance time.
- Time advances until the next important event, chosen stopping point, or manual stop.
- The player can configure how often Continue stops for routine reports.

Time units:

- **Hourly:** internal simulation tick for progress, support, infrastructure, and events.
- **Daily:** visible calendar movement, team progress, morale changes, candidate responses.
- **Weekly:** sprint summaries, market micro-adjustments, recruitment pool refresh.
- **Monthly:** payroll, revenue, finance report, product sales/users, competitor performance.
- **Quarterly:** major market reports, technology trend updates, optional investor/board review.
- **Yearly:** annual summary, market awards, macro events, product aging, long-term reputation.

## 02.4 Continue interruptions

Continue should stop for meaningful events:

- Product phase complete.
- Product ready for launch decision.
- Candidate responds to offer.
- Contract milestone or deadline.
- Monthly finance report.
- Cash runway falls below a configured threshold.
- Major competitor launch.
- Major infrastructure incident.
- Major bug/security/hardware defect issue.
- Team morale or burnout crisis.
- Research project complete.
- Market trend shift.
- Product launch day.

Routine minor events should go to the inbox without necessarily stopping Continue.

## 02.5 Player-configurable interruption filters

Players should be able to configure which events stop time:

- Critical only.
- Important and critical.
- All reports.
- Custom filters by category.

First playable can ship with a simpler default: critical and decision-required events stop Continue.

## 02.6 Session rhythm

A good 20-minute play session should include:

- Reviewing a current problem.
- Making one or more personnel/product decisions.
- Advancing time.
- Receiving at least one meaningful report.
- Seeing measurable company progress.

A good multi-hour session should include:

- Hiring changes.
- Product milestones.
- Market movement.
- Launch or crisis events.
- Financial pressure and recovery.
- Long-term strategic repositioning.

---

# GDD_03 — Founder, Company Creation & Sandbox Setup

## 03.1 Purpose

Company creation establishes the player fantasy, starting constraints, initial identity, and sandbox rules.

## 03.2 Company creation inputs

The player creates:

- Founder name.
- Company name.
- Company logo or icon.
- Company colour.
- Starting location/region.
- Founder background.
- Starting capital preset.
- Company focus.
- Sandbox difficulty.
- Market generation seed.

## 03.3 Founder backgrounds

Founder background should affect starting conditions without locking the player’s future strategy.

Recommended backgrounds:

- **Engineer:** better technical evaluation, lower early hiring uncertainty for engineers.
- **Product Designer:** better UX/design evaluation and slightly better early product concept quality.
- **Sales Founder:** better early contract access and business-development hiring evaluation.
- **Hardware Specialist:** lower early prototype risk for hardware products.
- **Research Founder:** stronger research project efficiency, weaker early sales access.
- **Serial Founder:** better investor confidence and hiring reputation, higher expectations.
- **Bootstrapped Founder:** lower starting cash but better cost discipline and ownership control.

Prototype implementation may represent these as simple starting bonuses.

## 03.4 Company focus

Company focus is a starting preference and identity label. It does not permanently gate content.

Suggested focus options:

- Consumer Software.
- Enterprise SaaS.
- Developer Tools.
- Games & Entertainment.
- Hardware Devices.
- Cloud Infrastructure.
- Security.
- AI & Automation.
- Platform Ecosystems.

The focus can influence starting market knowledge and initial generated opportunities, but the player can pivot.

## 03.5 Starting capital presets

Prototype values:

- **Garage Start:** £35,000, high pressure.
- **Bootstrapped Startup:** £75,000, default.
- **Seed Funded:** £250,000, easier early hiring.
- **Venture Start:** £1,000,000, fast growth but higher expectations.
- **Sandbox:** configurable.

Currency symbol is a UI/localisation setting. This GDD uses pounds for prototype examples.

## 03.6 Starting world options

Sandbox setup should allow:

- Market size: small, standard, large.
- Competitor density: low, standard, high.
- Technology pace: slow, standard, fast.
- Economic volatility: calm, standard, chaotic.
- Hiring market difficulty: easy, standard, hard.
- Hardware complexity: simplified, standard, advanced.
- Failure mode: forgiving, standard, hardcore.

First playable can reduce this to a small set of presets.

## 03.7 Starting state

Default first playable starting state:

- Company exists but has no launched products.
- Founder is active but not represented as a direct worker by default.
- Cash starts at selected preset.
- Initial candidate pool exists.
- Initial contract board exists.
- Initial market report exists.
- No teams exist until the player creates one.
- Tutorial hints can guide the first hire/team/product.

---

# GDD_04 — Employees & Hiring

## 04.1 Purpose

Employees are the company’s capability base. Hiring is one of the player’s most important recurring decisions.

Employees should feel like people with strengths, costs, preferences, and risks, not interchangeable production units.

## 04.2 Employee data

Each employee has:

- Name.
- Role.
- Seniority.
- Salary.
- Skills.
- Specialisations.
- Potential.
- Traits.
- Morale.
- Burnout risk.
- Loyalty.
- Ambition.
- Work preference.
- Team compatibility.
- Employment history.
- Current team.
- Current assignment through team.

## 04.3 Core roles

Recommended role list:

- Software Engineer.
- Hardware Engineer.
- Product Designer.
- Product Manager.
- QA Engineer.
- Infrastructure Engineer.
- Researcher.
- Data/AI Specialist.
- Marketing Specialist.
- Sales / Business Development.
- Support Specialist.
- Operations / Supply Chain Specialist.
- Recruiter / HR Specialist.
- Finance / Admin Specialist.
- Team Lead.

First playable recommended roles:

- Software Engineer.
- Hardware Engineer.
- Product Designer.
- QA Engineer.
- Marketing Specialist.
- Support Specialist.
- Infrastructure Engineer.
- Product Manager.

## 04.4 Skill model

Use readable 0–100 skill values.

Core skill categories:

- Engineering.
- Hardware.
- Design.
- QA/Reliability.
- Infrastructure.
- Research.
- Marketing.
- Sales.
- Support.
- Operations.
- Leadership.
- Collaboration.

A role should have primary and secondary skill weights. For example:

- Software Engineer: Engineering primary, QA and Collaboration secondary.
- Hardware Engineer: Hardware primary, QA and Operations secondary.
- Product Designer: Design primary, Product sense and Collaboration secondary.
- Product Manager: Product sense primary, Leadership and Collaboration secondary.

## 04.5 Employee traits

Traits add personality and decision texture.

Examples:

- Fast Learner.
- Detail-Oriented.
- Visionary.
- Pragmatic.
- Independent.
- Team Player.
- Conflict-Prone.
- Burnout-Prone.
- Mentor.
- Startup Veteran.
- Enterprise Experience.
- Hardware Lab Specialist.
- Growth Hacker.
- Security-Minded.

Traits should be limited in number per employee to keep profiles readable.

## 04.6 Hiring pipeline

Hiring flow:

1. Player identifies a need.
2. Player opens Recruitment Hub.
3. Player creates a job post or searches candidate pool.
4. Candidates appear with partial information.
5. Player reviews profiles, interviews, or shortlists.
6. Player sends an offer.
7. Candidate accepts, rejects, or negotiates.
8. Accepted candidate joins after notice period.
9. Candidate is assigned to a team.

## 04.7 Job post fields

The player can define:

- Role.
- Seniority.
- Salary range.
- Required skills.
- Preferred skills.
- Team assignment target.
- Urgency.
- Remote/on-site preference if location is used.
- Company pitch.
- Hiring budget.

## 04.8 Candidate information uncertainty

The game should not reveal everything perfectly before hiring.

Known by default:

- Role.
- Salary expectation.
- Seniority.
- Recent employer type.
- Visible skills.
- Availability.

Revealed through interview/reference/checks:

- Potential estimate.
- Trait confidence.
- Team compatibility.
- Burnout risk.
- Loyalty risk.
- Hidden strengths/weaknesses.

## 04.9 Offer acceptance

Offer acceptance should depend on:

- Salary compared to expectation.
- Company reputation.
- Product excitement.
- Role fit.
- Career growth.
- Candidate ambition.
- Candidate risk tolerance.
- Competition from other employers.

The game should explain rejection reasons in reports without exposing every formula.

## 04.10 Morale and burnout

Morale is affected by:

- Workload.
- Team compatibility.
- Salary satisfaction.
- Product success/failure.
- Crunch/overtime.
- Leadership.
- Company stability.
- Layoffs or crises.

Burnout risk is affected by:

- Sustained overwork.
- High-pressure launches.
- Crisis work.
- Poor team fit.
- Lack of rest after crunch.

Burnout should reduce productivity and increase resignation risk.

## 04.11 Training and growth

Employees gain experience through:

- Product work.
- Contract work.
- Mentoring.
- Formal training.
- Research projects.
- Post-launch support.

Product work should generally provide deeper growth than low-risk contract work.

## 04.12 Resignation and retention

Employees can leave if:

- Salary is too low.
- Morale is consistently low.
- Burnout is high.
- Company instability is severe.
- A competitor poaches them.
- Career growth is blocked.

Retention tools:

- Salary increase.
- Promotion.
- Better team placement.
- Lower workload.
- Training.
- Stock/equity packages if implemented later.

---

# GDD_05 — Teams & Assignments

## 05.1 Purpose

Teams convert employee capability into company output. The player should think in terms of team composition, team identity, workload, and assignment strategy.

## 05.2 Team creation

The player can create a team by selecting:

- Team name.
- Team function.
- Initial members.
- Optional team lead.
- Priority focus.

Team functions:

- Core Software.
- Hardware Lab.
- Product Design.
- QA & Reliability.
- Infrastructure.
- Marketing.
- Support.
- Research.
- Growth / Sales.
- Operations.

First playable can use flexible teams rather than strict department-only teams.

## 05.3 Team data

Each team has:

- Name.
- Function.
- Members.
- Lead.
- Skill coverage.
- Capacity.
- Morale.
- Cohesion.
- Workload.
- Current assignment.
- Assignment history.
- Specialisation tags.
- Recent output.

## 05.4 Assignment rule

Default rule:

- A team can have one primary assignment at a time.
- A team can support limited secondary duties only if explicitly configured.
- Overassignment creates context-switching penalties and burnout risk.

First playable recommended rule:

- One team, one assignment.

This keeps the game readable.

## 05.5 Team fit

Team fit is calculated from:

- Required roles covered.
- Relevant skill levels.
- Seniority mix.
- Team cohesion.
- Leadership.
- Tooling/infrastructure.
- Workload.
- Prior experience with this product category.

## 05.6 Team cohesion

Cohesion improves when:

- Members work together over time.
- Team has stable leadership.
- Team completes work successfully.
- Workload is reasonable.

Cohesion decreases when:

- Members frequently change.
- Team repeatedly fails milestones.
- Workload is excessive.
- Team members have poor compatibility.

## 05.7 Team lead

A Team Lead improves coordination and reduces risk. A team can function without a lead, but larger teams should benefit from one.

Team Lead effects:

- Better coordination.
- Reduced context switching.
- Improved morale stability.
- Better reporting accuracy.
- Reduced schedule slip risk.

Team Leads are recommended but not required in the first playable build.

## 05.8 Assignment types

Teams can be assigned to:

- Product research.
- Product concept.
- Product design.
- Software development.
- Hardware prototyping.
- QA/testing.
- Launch preparation.
- Post-launch support.
- Product updates.
- Marketing campaign.
- Infrastructure scaling.
- Contract work.
- Research project.
- Manufacturing setup.
- Crisis response.

## 05.9 Progress formula concept

Prototype progress formula:

`Daily Progress = Base Team Capacity × Role Coverage × Skill Fit × Cohesion × Morale × Tooling Modifier × Workload Modifier × Complexity Modifier`

This formula should not be shown directly to players. The UI should show readable summaries:

- Strong role coverage.
- Weak QA coverage.
- Low morale is slowing progress.
- Tooling is insufficient for this product scale.
- Team is overloaded.

## 05.10 Team planner

A Team Capacity Planner should show:

- Teams.
- Current assignments.
- Availability.
- Workload.
- Role gaps.
- Skill gaps.
- Burnout risk.
- Product dependencies.
- Upcoming deadlines.

This is one of the most important UI screens.

---

# GDD_06 — Products Overview

## 06.1 Purpose

Products are the main long-term output of the company. They create revenue, reputation, support burden, ecosystem strength, and market position.

Products should feel like businesses, not one-time projects.

## 06.2 Product families

Product families:

- Software products.
- Hardware products.
- Hybrid products.
- Platform products.
- Internal tools.
- Client contract products.

## 06.3 Product lifecycle

Default lifecycle:

1. Idea.
2. Market Research.
3. Concept.
4. Prototype.
5. Planning.
6. Development.
7. QA / Validation.
8. Launch Preparation.
9. Launch.
10. Post-Launch Support.
11. Updates / Expansion.
12. Maturity.
13. Decline.
14. Sunset / Successor.

First playable can simplify to:

1. Concept.
2. Development.
3. QA.
4. Launch.
5. Support / Update.

## 06.4 Product creation inputs

When creating a product, the player chooses:

- Product name.
- Product family.
- Product type.
- Target market segment.
- Customer segment.
- Price model.
- Feature scope.
- Quality target.
- Release window.
- Supported platforms.
- Required teams.
- Marketing strategy.
- Infrastructure plan.
- Support plan.

## 06.5 Product quality dimensions

All products use these broad quality dimensions, weighted differently by type:

- Feature depth.
- Usability.
- Reliability.
- Performance.
- Security.
- Compatibility.
- Brand appeal.
- Innovation.
- Price/value.
- Support quality.

Hardware products also use:

- Build quality.
- Component quality.
- Manufacturing readiness.
- Defect rate.
- Battery/thermal/physical design where relevant.

## 06.6 Product status

Statuses:

- Draft.
- In research.
- In concept.
- In prototype.
- In development.
- In QA.
- Ready for launch.
- Launched.
- Updating.
- Supported.
- Mature.
- Declining.
- Sunset.
- Cancelled.

## 06.7 Product success factors

Product performance depends on:

- Product quality.
- Market fit.
- Pricing.
- Marketing.
- Brand reputation.
- Launch timing.
- Competitor strength.
- Support quality.
- Infrastructure reliability.
- Ecosystem synergy.
- Hardware availability or software scalability.

## 06.8 Product reporting

Each product should have:

- Product overview page.
- Development progress page.
- Quality/risk page.
- Financial performance page.
- User/customer metrics page.
- Support/bugs page.
- Competitor comparison page.
- Update history.

## 06.9 Product cancellation

The player can cancel a product before launch.

Effects:

- Stops future cost.
- Frees teams.
- May hurt morale if the team worked long on it.
- May hurt reputation if publicly announced.
- May preserve cash and avoid worse outcome.

First playable can use simple morale and sunk-cost reporting.

---

# GDD_07 — Software Products

## 07.1 Purpose

Software products are lower physical-risk products that can scale rapidly but require support, infrastructure, updates, and market fit.

## 07.2 Software product types

Recommended software types:

- Website / Web Platform.
- Productivity App.
- Business SaaS.
- Developer Tool.
- Game.
- Operating System.
- Cloud Service.
- Security Software.
- AI / Data Tool.
- Marketplace / App Store.
- Communication Tool.
- Creative Tool.

First playable software types:

- Web Platform.
- Productivity App.
- Business SaaS.
- Developer Tool.
- Game.

## 07.3 Software revenue models

Revenue models:

- One-time purchase.
- Subscription.
- Freemium.
- Enterprise licence.
- Usage-based billing.
- Ad-supported.
- Marketplace commission.

First playable recommended models:

- One-time purchase.
- Subscription.
- Freemium.

## 07.4 Software product attributes

Software products track:

- Active users.
- New users.
- Churn.
- Revenue.
- Infrastructure load.
- Uptime.
- Bug count.
- Security risk.
- Support tickets.
- Feature satisfaction.
- Review score.
- Competitor comparison.

## 07.5 Software pressures

Software pressure sources:

- Bugs.
- Server load.
- Security vulnerabilities.
- Compatibility issues.
- User churn.
- Feature requests.
- Competitor feature launches.
- Market trend changes.
- Support backlog.

## 07.6 Software development phases

Suggested phase weights:

- Concept: Product Manager, Designer, Researcher.
- Development: Software Engineer, Product Designer, QA Engineer.
- QA: QA Engineer, Software Engineer, Support Specialist.
- Launch Prep: Marketing Specialist, Infrastructure Engineer, Support Specialist.
- Support: Support Specialist, QA Engineer, Software Engineer.
- Update: Software Engineer, Product Designer, QA Engineer.

## 07.7 Infrastructure requirement

Online software products require infrastructure capacity. Infrastructure affects:

- Uptime.
- Latency.
- User retention.
- Support tickets.
- Security incidents.
- Operating cost.

If infrastructure utilisation exceeds safe limits, outage risk rises.

## 07.8 Software update system

Updates can improve:

- Features.
- Usability.
- Reliability.
- Performance.
- Security.
- Compatibility.

Updates can also introduce new bugs. QA investment reduces update risk.

## 07.9 Software end-of-life

A software product can be sunset.

Effects:

- Stops major support cost over time.
- May reduce reputation if users remain active.
- May push users to successor product if ecosystem migration is handled well.

---

# GDD_08 — Hardware Products

## 08.1 Purpose

Hardware products are higher-risk, higher-capital products. They require prototyping, manufacturing readiness, component planning, inventory, defect management, and support.

Hardware should feel meaningfully different from software.

## 08.2 Hardware product types

Recommended hardware types:

- Laptop.
- Desktop Computer.
- Smartphone.
- Tablet.
- Wearable Device.
- Game Console.
- Server Hardware.
- Peripheral.
- Smart Home Device.
- Development Kit.
- Custom Chip / Processor.

First playable hardware types:

- Peripheral.
- Laptop/Desktop Device.
- Server Device.

These are easier to abstract than full phones or consoles.

## 08.3 Hardware lifecycle

Hardware lifecycle:

1. Market Research.
2. Industrial/Product Design.
3. Component Selection.
4. Prototype.
5. Validation.
6. Manufacturing Setup.
7. Launch Stock Production.
8. Launch.
9. Warranty/Support.
10. Revision.
11. Successor Model.
12. End-of-Life.

First playable can simplify to:

1. Concept.
2. Prototype.
3. Manufacturing Prep.
4. Launch.
5. Support.

## 08.4 Hardware attributes

Hardware tracks:

- Performance.
- Build quality.
- Reliability.
- Design appeal.
- Manufacturing cost.
- Unit margin.
- Defect rate.
- Warranty cost.
- Launch stock.
- Inventory.
- Component availability.
- Compatibility.
- Review score.
- Return rate.

## 08.5 Bill of materials

Each hardware product has a simplified bill of materials.

BOM affects:

- Unit cost.
- Performance.
- Reliability.
- Manufacturing difficulty.
- Supply risk.
- Price flexibility.

First playable can use abstract tiers rather than individual components.

Example component tiers:

- Budget.
- Standard.
- Premium.
- Experimental.

## 08.6 Manufacturing model

Manufacturing options:

- Outsourced manufacturing.
- Partner manufacturing.
- In-house manufacturing. Deferred.

First playable should use outsourced manufacturing with simple cost, capacity, and defect-rate variables.

## 08.7 Inventory and stock

Hardware requires stock.

The player chooses launch production quantity or manufacturing budget. Too little stock limits sales. Too much stock locks cash and risks obsolete inventory.

Stock states:

- In production.
- Available stock.
- Backordered.
- Obsolete stock.

## 08.8 Defects and recalls

Hardware products can have defects.

Defects create:

- Support tickets.
- Warranty costs.
- Return costs.
- Reputation risk.
- Possible recall events.

A recall is a major crisis event and should be rare in first playable.

## 08.9 Hardware/software synergy

Hardware can gain value from related software:

- Device + operating system.
- Console + games.
- Wearable + health app.
- Server hardware + cloud management tools.
- Development kit + developer tools.

This is central to the product ecosystem system.

---

# GDD_09 — Product Ecosystems & Platform Strategy

## 09.1 Purpose

Product ecosystems are Tech Mogul’s signature system. Products can connect, reinforce each other, and create long-term strategic advantages.

## 09.2 Ecosystem graph

Products can have relationships:

- Depends on.
- Supports.
- Integrates with.
- Bundles with.
- Drives users to.
- Shares infrastructure with.
- Competes with.
- Replaces.

Example:

- Developer Tool supports Operating System.
- Operating System supports Marketplace.
- Marketplace supports third-party apps.
- Hardware Device depends on Operating System.
- Cloud Service supports Business SaaS.

## 09.3 Ecosystem strength

Ecosystem strength is affected by:

- Number of connected active products.
- Quality of connected products.
- Compatibility.
- Developer/customer adoption.
- Marketplace activity.
- Support quality.
- Brand trust.
- Stability of platform APIs.

## 09.4 Ecosystem benefits

Strong ecosystems can create:

- Higher user retention.
- Better product adoption.
- Stronger brand perception.
- Lower marketing cost for related products.
- Higher switching costs.
- Marketplace/platform revenue.
- Better developer/customer loyalty.

## 09.5 Ecosystem risks

Risks:

- A weak platform product harms connected products.
- Infrastructure outages affect multiple products.
- Security issues spread reputation damage.
- Compatibility failures increase support burden.
- Overdependence on one ecosystem creates strategic vulnerability.

## 09.6 Platform products

Platform products include:

- Operating System.
- Cloud Platform.
- Marketplace / App Store.
- Game Console.
- Developer Platform.
- Hardware Device Family.

Platform products should be harder to build but more strategically powerful.

## 09.7 Third-party ecosystem support

Deferred advanced system:

- Developers or partners can build on the player’s platform.
- Platform adoption grows through developer tools, documentation, revenue share, and platform stability.
- Poor platform support reduces third-party interest.

First playable can simulate this as an abstract ecosystem adoption score.

## 09.8 Example strategy paths

### Developer ecosystem

Build a developer tool, then a platform, then a marketplace.

### Enterprise SaaS ecosystem

Build a business SaaS product, support tools, enterprise sales, integrations, and cloud infrastructure.

### Gaming ecosystem

Build a game, game engine or developer tool, then hardware or platform services.

### Consumer hardware ecosystem

Build a hardware device, companion app, subscription service, and accessories.

---

# GDD_10 — Market, Competitors & Trends

## 10.1 Purpose

The market gives context to player decisions. Competitors make the world feel alive. Trends prevent every save from playing the same way.

## 10.2 Market segments

Customer segments:

- Consumers.
- Small businesses.
- Enterprises.
- Developers.
- Gamers.
- Creators.
- Education.
- Government.
- Hardware enthusiasts.
- Budget buyers.
- Premium buyers.

Each segment has preferences for:

- Price.
- Reliability.
- Features.
- Usability.
- Brand.
- Security.
- Performance.
- Support.
- Ecosystem.

## 10.3 Market categories

Market categories map to product types:

- Web platforms.
- Productivity apps.
- Business SaaS.
- Developer tools.
- Games.
- Cloud services.
- Security software.
- Operating systems.
- Hardware devices.
- Server hardware.
- Gaming platforms.

## 10.4 Market state

Each market category tracks:

- Total demand.
- Growth rate.
- Customer preferences.
- Competitive intensity.
- Current leaders.
- Technology expectations.
- Price sensitivity.
- Marketing sensitivity.
- Support expectations.
- Trend modifiers.

## 10.5 Competitor companies

Competitors have:

- Name.
- Archetype.
- Cash strength.
- Product portfolio.
- Market focus.
- Reputation.
- Hiring strength.
- Research strength.
- Launch cadence.
- Risk appetite.
- Pricing style.
- Marketing style.

## 10.6 Competitor archetypes

Suggested archetypes:

- Incumbent Giant: high cash, slow but powerful.
- Aggressive Startup: fast launches, risk-taking, fragile finances.
- Research Lab: strong innovation, slower commercial execution.
- Hardware Manufacturer: strong physical products, weaker software.
- Enterprise Specialist: strong sales/support, less consumer appeal.
- Consumer Brand: strong marketing and design.
- Low-Cost Competitor: price pressure, lower quality ceiling.
- Platform Holder: ecosystem power, high switching costs.

## 10.7 Competitor actions

Competitors can:

- Launch products.
- Update products.
- Change prices.
- Run marketing campaigns.
- Hire talent.
- Poach employees.
- Enter or exit markets.
- Respond to trends.
- Retire products.
- Announce platforms.

First playable should implement simple competitor product launches, market share movement, and competitor summaries.

## 10.8 Trends

Trend examples:

- Remote work boom.
- AI automation demand.
- Security panic.
- Consumer hardware refresh cycle.
- Developer tooling shift.
- Cloud cost spike.
- Recession.
- Chip shortage.
- Subscription fatigue.
- Open-source surge.
- Gaming boom.
- Enterprise compliance wave.

Trends affect demand, costs, customer preferences, and product expectations.

## 10.9 Market reports

Market reports should show:

- Category growth.
- Key competitors.
- Product rankings.
- Average price.
- Customer preference shifts.
- Opportunity/risk indicators.
- Recent launches.
- Player company position.

Reports should show data, not direct instructions.

---

# GDD_11 — Finance, Revenue & Runway

## 11.1 Purpose

Finance creates pressure and gives management decisions consequences. The player must understand cash, burn, revenue, costs, and runway at all times.

## 11.2 Core finance variables

Track:

- Cash.
- Monthly payroll.
- Monthly infrastructure cost.
- Monthly support cost.
- Marketing spend.
- Research spend.
- Hardware manufacturing cost.
- Product revenue.
- Contract revenue.
- Profit/loss.
- Runway.
- Tax/accounting abstraction. Deferred.

## 11.3 Runway

Runway is the estimated time before cash reaches zero.

Prototype formula:

`Runway Months = Current Cash / Monthly Net Burn`

If net burn is zero or profitable, show “profitable” or “runway stable” instead of a number.

## 11.4 Payroll

Payroll is calculated monthly from employee salaries.

Payroll should be one of the clearest recurring costs.

Late-game can include bonuses, equity, benefits, and severance. First playable should focus on salary.

## 11.5 Software revenue

Software revenue depends on model.

One-time purchase:

`Revenue = New Units Sold × Price`

Subscription:

`Revenue = Active Subscribers × Monthly Price`

Freemium:

`Revenue = Active Users × Conversion Rate × Paid Price + Optional Ads`

Usage-based:

`Revenue = Usage Units × Usage Price`

First playable can use one-time and subscription.

## 11.6 Hardware revenue

Hardware revenue:

`Revenue = Units Sold × Price`

Hardware gross margin:

`Gross Margin = Price - Unit Manufacturing Cost - Warranty Reserve`

Hardware stock limits sales.

## 11.7 Contract revenue

Contracts may pay:

- Upfront.
- At milestones.
- On completion.
- With quality bonuses.
- With penalties for late/poor delivery.

First playable can use completion payment plus optional deadline bonus.

## 11.8 Expenses

Expenses include:

- Payroll.
- Infrastructure.
- Marketing.
- Research.
- Hardware prototyping.
- Manufacturing.
- Support.
- Recruiting.
- Office/admin overhead. Deferred or simplified.

## 11.9 Funding

Funding is thematically important for a startup game but can complicate the first version.

Funding options:

- Bootstrapping.
- Bank loan.
- Angel investment.
- Seed round.
- Venture round.
- Revenue-based financing.
- Strategic partnership.

Recommended status:

- Basic loans/investment are deferred until after core product/company loop works.
- First playable can use starting capital presets and contract/product revenue only.

## 11.10 Financial reports

Monthly finance report should include:

- Cash at start/end of month.
- Revenue by source.
- Expenses by category.
- Net profit/loss.
- Runway.
- Payroll change.
- Product revenue summary.
- Contract income.
- Infrastructure/support cost.

The report should support drill-down to product, employee, and cost pages.

---

# GDD_12 — Contracts & Short-Term Work

## 12.1 Purpose

Contracts provide early cash, training, and short-term decision pressure. They keep idle teams useful and give the player survival options before product revenue is stable.

Contracts must not replace owned products as the main long-term growth path.

## 12.2 Contract types

Software contracts:

- Website build.
- Mobile app prototype.
- Internal business tool.
- Cloud migration.
- Security audit.
- Data dashboard.
- QA/testing support.

Hardware contracts:

- Hardware prototype.
- Device testing.
- Manufacturing consultation.
- Peripheral design.
- Server setup.

Business contracts:

- Marketing campaign.
- Enterprise integration.
- Support outsourcing.
- Research study.

## 12.3 Contract data

Each contract has:

- Client name.
- Contract type.
- Required skills.
- Difficulty.
- Deadline.
- Payment.
- Milestones.
- Quality target.
- Reputation impact.
- Penalties.
- Optional bonus.

## 12.4 Contract flow

1. Contract appears on board.
2. Player reviews requirements.
3. Player assigns team.
4. Team progresses over time.
5. Milestones resolve.
6. Quality and deadline determine outcome.
7. Payment and report arrive.

## 12.5 Contract success factors

Success depends on:

- Team skill fit.
- Time allocated.
- Difficulty.
- Team morale.
- Workload.
- Role coverage.
- Scope changes/events.

## 12.6 Contract outcomes

Possible outcomes:

- Excellent delivery: full payment plus bonus and client reputation.
- Accepted delivery: full payment.
- Late delivery: reduced payment and possible reputation hit.
- Poor delivery: partial payment and client dissatisfaction.
- Failed delivery: no or minimal payment.

First playable can use simple excellent/accepted/failed outcomes.

## 12.7 Contract board

Contract board filters:

- Type.
- Difficulty.
- Deadline.
- Payment.
- Required roles.
- Client segment.
- Reputation requirement.

---

# GDD_13 — Research & Technology Progression

## 13.1 Purpose

Research gives long-term direction and progression without forcing a linear campaign.

Research should unlock new options and increase the ceiling for product quality.

## 13.2 Research tracks

Recommended tracks:

- Software Engineering.
- Hardware Engineering.
- Cloud Infrastructure.
- Security.
- AI & Automation.
- UX/Product Design.
- Developer Tools.
- Manufacturing.
- Marketing Analytics.
- Support Operations.
- Platform Ecosystems.

## 13.3 Research project data

Each research project has:

- Name.
- Track.
- Required skill.
- Duration.
- Cost.
- Unlocks.
- Risk level.
- Obsolescence risk.
- Required prerequisites.

## 13.4 Unlock types

Research can unlock:

- New product types.
- New features.
- Better infrastructure.
- Better hardware components.
- Better manufacturing methods.
- Better QA methods.
- Better security practices.
- Better marketing analytics.
- New revenue models.
- Ecosystem/platform features.

## 13.5 Technology obsolescence

Technology can become old over time. Obsolescence should affect:

- Customer expectations.
- Product review scores.
- Competitor advantage.
- Maintenance cost.
- Hardware attractiveness.

The player should receive reports when important technologies are aging.

## 13.6 Research visibility

Research UI should show:

- Available projects.
- Locked future projects.
- Requirements.
- Expected benefits.
- Assigned teams.
- Completion estimate.
- Related products.

The UI should not reveal exact hidden formulas but should explain why research matters.

## 13.7 First playable research scope

First playable should include:

- Simple research projects.
- Unlocking product features.
- Unlocking product types.
- Improving infrastructure or QA.

Advanced tech trees and patents are deferred.

---

# GDD_14 — Inbox, Reports & Founder Portal

## 14.1 Purpose

The Founder Portal is the command centre. It combines inbox, tasks, calendar, company health, active work, and important data.

For a UI-based game, this is the main “play space.”

## 14.2 Portal layout

Recommended portal areas:

- Top company status bar.
- Inbox/task feed.
- Calendar snapshot.
- Cash/runway card.
- Active products card.
- Team workload card.
- Hiring pipeline card.
- Market/competitor news card.
- Infrastructure/support card.
- Upcoming decisions card.

## 14.3 Top company status bar

Always visible:

- Current date.
- Continue button.
- Cash.
- Runway.
- Monthly profit/loss.
- Active alerts count.
- Search.
- Main navigation.

## 14.4 Inbox categories

Categories:

- All.
- Requires Decision.
- Finance.
- Products.
- Employees.
- Teams.
- Hiring.
- Market.
- Competitors.
- Infrastructure.
- Support.
- Contracts.
- Research.
- Archived.

## 14.5 Report types

Reports:

- Monthly finance report.
- Product progress report.
- Product launch report.
- Sales/user report.
- Support/bug report.
- Candidate report.
- Offer response.
- Team morale report.
- Market trend report.
- Competitor launch report.
- Infrastructure incident report.
- Hardware defect report.
- Marketing campaign report.
- Research completion report.
- Contract completion report.

## 14.6 Report structure

Each report should include:

- Title.
- Date.
- Category.
- Summary.
- Key numbers.
- What changed.
- Cause indicators.
- Related entities.
- Available actions.

Reports should explain outcomes, not prescribe perfect solutions.

Allowed report wording:

- “Cash runway fell from 5.2 months to 3.8 months.”
- “Support backlog increased after the latest update.”
- “Product quality risk is high due to weak QA coverage.”

Avoid report wording:

- “You should hire more QA.”
- “Cancel this product.”
- “This is the best strategy.”

## 14.7 Inbox actions

Actions:

- Open.
- Archive.
- Delete.
- Pin.
- Mark unread.
- Add reminder.
- Jump to related product/team/employee/market.
- Create task from report.

First playable should include open, archive, delete, and navigation links.

## 14.8 Search and bookmarks

The game should support:

- Global search across employees, products, teams, reports, and markets.
- Player bookmarks for frequently used screens.
- Recent pages.

This is important for long saves.

---

# GDD_15 — Events, Crises & Launch Moments

## 15.1 Purpose

Events create peaks in an otherwise strategic, data-driven game. They should make preparation matter.

Major product launches should feel like management “event days.”

## 15.2 Event categories

- Product launch.
- Major update release.
- Conference/showcase.
- Infrastructure outage.
- Security incident.
- Hardware defect wave.
- Recall.
- Competitor surprise launch.
- Employee resignation or poaching.
- Funding opportunity. Deferred.
- Economic shock.
- Supply shortage.
- Viral marketing moment.
- Review controversy.

## 15.3 Product launch event flow

Launch flow:

1. Launch readiness check.
2. Player confirms launch, delays, or cancels.
3. Initial sales/users generated.
4. Reviews and sentiment arrive.
5. Server/support load appears.
6. Competitor and market reaction occurs.
7. Player may respond with support, patch, marketing, server scaling, or price adjustment.
8. Launch report summarises outcome.

## 15.4 Launch readiness indicators

Show:

- Quality readiness.
- QA confidence.
- Bug risk.
- Infrastructure readiness.
- Support readiness.
- Marketing readiness.
- Market timing.
- Competitor pressure.
- Cash risk.

These indicators should be descriptive, not deterministic.

## 15.5 Crisis response

Crisis events should give the player options such as:

- Assign team to urgent fix.
- Increase support capacity.
- Scale infrastructure.
- Issue public apology.
- Delay launch/update.
- Absorb cost.
- Recall hardware.
- Replace manufacturing partner.
- Reduce product scope.

Each response has cost, time, morale, and reputation consequences.

## 15.6 Event frequency

Events must not become spam.

Guideline:

- Early game: rare, clear events.
- Mid game: occasional complex events.
- Late game: more events, but filtered by report priority and delegation systems.

---

# GDD_16 — UI Structure

## 16.1 Purpose

The UI is the game. It must prioritise clarity, speed, and findability before visual flourish.

## 16.2 Core UX principles

- Common actions should be reachable quickly.
- Every important number should have context.
- Every alert should have a related screen link.
- Tables need sorting and filtering.
- Long lists need search.
- Reports need clear summaries.
- Players must be able to return to the portal easily.
- The UI should show data first, recommendations second or not at all.
- Avoid hidden nested menus for core tasks.

## 16.3 Main navigation

Main sections:

- Portal.
- Company.
- Employees.
- Recruitment.
- Teams.
- Products.
- Contracts.
- Research.
- Infrastructure.
- Market.
- Competitors.
- Finance.
- Reports.
- Calendar.
- Settings.

## 16.4 Key screens

### Portal screen

Command centre with inbox, tasks, calendar, cash, products, teams, hiring, market, and alerts.

### Company screen

Company profile, reputation, focus, current goals, milestones, and history.

### Employees screen

Employee list, filters, profiles, morale, salary, team, role, skills, risk.

### Recruitment Hub

Job posts, candidates, shortlists, offers, hiring analytics.

### Teams screen

Team list, team profiles, capacity, workload, cohesion, assignments.

### Products screen

Product portfolio, status, revenue, users, quality, support, roadmap.

### Product detail screen

Deep product page with phases, teams, risks, finance, market position, support, updates.

### Roadmap Visualiser

Shows product plan, role coverage, timeline, risk, support, infrastructure, market timing.

### Market screen

Market categories, demand, growth, trends, customer segments, rankings.

### Competitors screen

Competitor companies and competitor product pages.

### Finance screen

Cash, revenue, expenses, runway, product P&L, monthly reports.

## 16.5 Roadmap Visualiser

The Roadmap Visualiser is a key planning tool.

It should show:

- Product phases.
- Assigned teams.
- Role coverage.
- Skill gaps.
- Estimated timeline.
- Quality risk.
- Budget pressure.
- Support burden.
- Infrastructure load.
- Market timing.
- Competitor pressure.

The visualiser should help the player understand consequences without prescribing the best answer.

## 16.6 Accessibility

Required accessibility goals:

- Scalable text.
- Keyboard navigation support for main UI.
- Colourblind-safe status indicators.
- Tooltips for terms.
- Clear contrast.
- Avoid relying on colour alone.
- Pause by default during decision-making.

## 16.7 Data display standards

Every numeric card should show:

- Current value.
- Direction/trend.
- Timeframe.
- Source or drill-down link.

Example:

- Cash: £82,400, down £12,100 this month, view finance report.
- Active users: 21,400, up 8% this month, view product analytics.

---

# GDD_17 — Save/Load & Persistent Simulation

## 17.1 Purpose

Long sandbox saves require reliable persistence. The game must save the full state of the company and world simulation.

## 17.2 Save requirements

Save state includes:

- Current date/time.
- Company data.
- Founder data.
- Employees.
- Candidates.
- Teams.
- Products.
- Contracts.
- Research.
- Infrastructure.
- Finance history.
- Market state.
- Competitor state.
- Reports/inbox.
- Calendar events.
- Random seed/state.
- Player settings.

## 17.3 Autosave

Autosave triggers:

- Monthly boundary.
- Before product launch.
- Before major crisis resolution.
- Before manual Continue after major decision.
- On exit.

Player can configure autosave frequency.

## 17.4 Save slots

Recommended:

- Manual save slots.
- Rolling autosaves.
- Quick save.
- Named company saves.

## 17.5 Determinism

The simulation should be deterministic from saved state where possible. Reloading should not randomly change outcomes unless the player changes decisions.

Random generation should store seeds and generated values.

## 17.6 Save compatibility

For development:

- Include save version number.
- Support migrations for major updates where practical.
- Detect incompatible saves and warn clearly.

## 17.7 Data volume

Long saves may accumulate many reports and history entries. The game should support archiving and summarising older data.

Recommended:

- Keep full data for recent months.
- Compress older reports into summaries if necessary.
- Keep important milestones permanently.

---

# GDD_18 — First Playable Version Scope

## 18.1 Purpose

The first playable version should prove the core loop: hire, form teams, assign work, advance time, receive reports, launch products, manage cash, and react to market feedback.

## 18.2 MVP feature list

Required first playable features:

- Company creation.
- Founder/company profile.
- Founder Portal.
- Manual Continue time flow.
- Calendar date.
- Employees.
- Candidate pool.
- Basic hiring/offers.
- Teams.
- Team assignment.
- Contracts.
- Software products.
- Simplified hardware products.
- Product lifecycle.
- Product launch.
- Basic post-launch revenue/users.
- Basic support/bugs.
- Basic infrastructure capacity.
- Basic market categories.
- Basic competitors.
- Finance: cash, payroll, revenue, expenses, runway.
- Inbox/reports.
- Save/load.

## 18.3 MVP software scope

First playable software products:

- Web Platform.
- Productivity App.
- Business SaaS.
- Developer Tool.
- Game.

Revenue models:

- One-time purchase.
- Subscription.

## 18.4 MVP hardware scope

First playable hardware products:

- Peripheral.
- Laptop/Desktop Device.
- Server Device.

Hardware systems:

- Prototype.
- Manufacturing cost.
- Launch stock.
- Unit sales.
- Defect/support risk.

Deferred:

- Full supply chain.
- In-house manufacturing.
- Detailed components.
- Retail channels.
- Recalls, unless as rare scripted-style event.

## 18.5 MVP competitor scope

Competitors should:

- Exist in the market.
- Have products.
- Launch new products occasionally.
- Affect market share.
- Be visible on market pages.

Deferred:

- Deep competitor staffing.
- M&A.
- Detailed financial simulation.
- Advanced AI strategy.

## 18.6 MVP reports

Required reports:

- Monthly finance report.
- Candidate offer response.
- Contract completion report.
- Product progress milestone report.
- Product launch report.
- Monthly product performance report.
- Market trend report.
- Competitor launch report.
- Infrastructure warning report.
- Support/bug report.

## 18.7 MVP exclusions

Exclude from first playable:

- Full office construction.
- Physical employee simulation.
- Stock market.
- Acquisitions.
- Advanced investment/funding.
- Deep legal/regulatory systems.
- Patents.
- Multiplayer.
- Advanced modding.
- Campaign narrative.
- Founder life simulation.
- Full global market geography.

## 18.8 MVP success criteria

The first playable is successful if a player can:

1. Start a company.
2. Hire employees.
3. Create a team.
4. Accept a contract or create a product.
5. Assign a team.
6. Advance time.
7. Receive understandable progress reports.
8. Launch a product.
9. Earn or lose money based on visible causes.
10. Compare at least one competitor product.
11. Make a new decision based on reports.
12. Save and reload the game.

## 18.9 Recommended vertical slice

Vertical slice scenario:

- Player starts with £75,000.
- Player hires 3–5 employees.
- Player forms one software team.
- Player accepts one contract for early cash.
- Player creates a Web Platform or Productivity App.
- Product moves through concept, development, QA, launch.
- Competitor launches a rival product.
- Player receives product launch report and monthly finance report.
- Player chooses to update product, hire more staff, or start another product.

This proves the core loop without needing late-game complexity.

---

# Appendix A — Core Data Objects

## A.1 Company

Fields:

- Company ID.
- Name.
- Founder ID.
- Logo/icon.
- Colour.
- Starting focus.
- Current reputation.
- Cash.
- Founded date.
- Location.
- Employees.
- Teams.
- Products.
- Contracts.
- Research projects.
- Reports.
- Market positions.

## A.2 Employee

Fields:

- Employee ID.
- Name.
- Role.
- Seniority.
- Salary.
- Skills.
- Traits.
- Potential.
- Morale.
- Burnout.
- Loyalty.
- Current team.
- Employment status.
- Hire date.
- Performance history.

## A.3 Candidate

Fields:

- Candidate ID.
- Name.
- Role.
- Salary expectation.
- Visible skills.
- Hidden traits.
- Availability date.
- Interest level.
- Offer status.
- Confidence level.

## A.4 Team

Fields:

- Team ID.
- Name.
- Function.
- Members.
- Lead.
- Cohesion.
- Morale.
- Workload.
- Current assignment.
- Skill coverage.
- Assignment history.

## A.5 Product

Fields:

- Product ID.
- Name.
- Family.
- Type.
- Status.
- Target market.
- Customer segment.
- Price model.
- Price.
- Feature scope.
- Quality dimensions.
- Assigned teams.
- Development progress.
- Launch date.
- Users/customers.
- Revenue history.
- Support burden.
- Infrastructure load.
- Market share.
- Competitor comparisons.
- Ecosystem links.

## A.6 Contract

Fields:

- Contract ID.
- Client name.
- Type.
- Required skills.
- Difficulty.
- Deadline.
- Payment.
- Progress.
- Assigned team.
- Quality target.
- Status.
- Outcome.

## A.7 Market category

Fields:

- Category ID.
- Name.
- Demand.
- Growth rate.
- Customer preferences.
- Competitors.
- Product rankings.
- Trend modifiers.
- Price sensitivity.

## A.8 Report

Fields:

- Report ID.
- Date.
- Category.
- Title.
- Summary.
- Key values.
- Related entities.
- Required decision flag.
- Read/archive/delete state.

---

# Appendix B — First-Pass Balance Values

All values here are prototype values.

## B.1 Starting capital

- Garage Start: £35,000.
- Bootstrapped Startup: £75,000.
- Seed Funded: £250,000.
- Venture Start: £1,000,000.

## B.2 Salary bands

Prototype monthly salary ranges:

- Junior: £2,000–£3,500.
- Mid-level: £3,500–£6,000.
- Senior: £6,000–£10,000.
- Principal/Lead: £10,000–£16,000.

## B.3 Product timelines

Prototype minimum timelines:

- Small software product: 1–3 months.
- Medium software product: 3–8 months.
- Large software product: 8–18 months.
- Small hardware product: 3–6 months.
- Medium hardware product: 6–18 months.
- Platform product: 12+ months.

## B.4 Product complexity tiers

- Tier 1: simple internal/productivity/web product.
- Tier 2: commercial software or simple hardware.
- Tier 3: complex SaaS, developer tool, server product.
- Tier 4: operating system, console, cloud platform, advanced hardware.
- Tier 5: ecosystem-defining platform.

## B.5 Candidate response time

- Fast response: 1–3 days.
- Standard response: 4–10 days.
- Slow response: 11–21 days.

## B.6 Report frequency

- Finance: monthly.
- Product performance: monthly after launch.
- Market report: monthly or quarterly depending on importance.
- Team morale: on threshold or monthly summary.
- Candidate reports: event-driven.
- Contract reports: milestone/completion.

---

# Appendix C — Example Player Stories

## C.1 Early software startup

The player starts with £75,000, hires two software engineers and a designer, forms a Core Software Team, accepts a small website contract, then uses the cash to build a productivity app. The app launches with average reviews but strong retention. The player hires support and plans a subscription update.

## C.2 Hardware pivot

The player begins as a hardware specialist, builds a peripheral, underestimates manufacturing defects, and loses cash on warranty support. The player responds by hiring QA and operations staff, improving the next product revision, and bundling software to improve value.

## C.3 Platform company

The player builds a developer tool, then an operating system, then a marketplace. Early products are not highly profitable, but the ecosystem begins to reinforce itself. Competitors respond with price cuts and rival tools.

## C.4 Enterprise SaaS company

The player focuses on business customers, hires sales/support, builds SaaS with high reliability, and grows recurring revenue. The company becomes profitable slowly but defensibly.

## C.5 Crisis recovery

A major product update causes infrastructure outages and support backlog. The player pauses new development, assigns engineers to stability work, increases infrastructure capacity, and releases a corrective patch. The product’s growth slows but reputation damage is contained.

---

# Appendix D — Glossary

**Active users:** Users currently using a software product within a recent period.

**Assignment:** Work given to a team, such as product development, support, research, or contract work.

**Burn rate:** Monthly net cash loss.

**Candidate:** A potential employee before being hired.

**Cohesion:** How effectively a team works together over time.

**Continue:** Player command that advances time until the next important event or stopping point.

**Ecosystem:** A group of connected products that reinforce or depend on each other.

**Founder Portal:** Main command centre combining inbox, reports, tasks, calendar, and company state.

**Infrastructure:** Systems needed to run software products and internal tools, such as hosting, servers, build systems, support tooling, and security.

**Market fit:** How well a product matches customer demand and segment preferences.

**Product lifecycle:** The phases a product moves through from concept to support or sunset.

**Runway:** Estimated time before the company runs out of cash.

**Support burden:** Ongoing workload created by bugs, users, customers, defects, and service requirements.

---

# Appendix E — Source List

Reference sources consulted for genre analysis:

- Startup Company official site — https://www.startupcompanygame.com/
- Startup Company Steam page — https://store.steampowered.com/app/606800/Startup_Company/
- Software Inc. Steam page — https://store.steampowered.com/app/362620/Software_Inc/
- Software Inc. official site — https://softwareinc.coredumping.com/
- Football Manager 26 UI feature page — https://www.footballmanager.com/fm26/features/fm26s-reimagined-user-interface
- Football Manager 26 recruitment feature page — https://www.footballmanager.com/fm26/features/powered-transferroom-fm26s-recruitment-revamp
- Football Manager 26 tactical visualiser feature page — https://www.footballmanager.com/fm26/features/possession-out-possession-fm26s-new-tactical-evolution
- Game Dev Tycoon Steam page — https://store.steampowered.com/app/239820/Game_Dev_Tycoon/
- Mad Games Tycoon 2 Steam page — https://store.steampowered.com/app/1342330/Mad_Games_Tycoon_2/
- Computer Tycoon Steam page — https://store.steampowered.com/app/686680/Computer_Tycoon/
- Capitalism Lab official site — https://www.capitalismlab.com/
- Big Ambitions Steam page — https://store.steampowered.com/app/1331550/Big_Ambitions/
- Motorsport Manager Steam page — https://store.steampowered.com/app/415200/Motorsport_Manager/
- Motorsport Manager official page — https://www.playsportgames.com/games/motorsport-manager-pc/
