# Contributing to ONGC Milestone Event-Driven System

First off, thank you for considering contributing to the ONGC Milestone System! It's people like you that make this project better.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Commit Message Guidelines](#commit-message-guidelines)
- [Testing Guidelines](#testing-guidelines)

---

## Code of Conduct

This project and everyone participating in it is governed by respect, professionalism, and collaboration. By participating, you are expected to uphold these standards.

### Our Standards

- Use welcoming and inclusive language
- Be respectful of differing viewpoints and experiences
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

---

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates.

**How to Submit a Good Bug Report:**

- Use a clear and descriptive title
- Describe the exact steps to reproduce the problem
- Provide specific examples
- Describe the behavior you observed and what you expected
- Include screenshots if applicable
- Include your environment details (OS, .NET version, Node.js version, etc.)

**Bug Report Template:**

```markdown
**Description:**
A clear description of the bug

**Steps to Reproduce:**
1. Go to '...'
2. Click on '...'
3. See error

**Expected Behavior:**
What you expected to happen

**Actual Behavior:**
What actually happened

**Environment:**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., 8.0]
- Node.js Version: [e.g., 18.17.0]

**Screenshots:**
If applicable

**Additional Context:**
Any other relevant information
```

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues.

**How to Submit a Good Enhancement Suggestion:**

- Use a clear and descriptive title
- Provide a detailed description of the suggested enhancement
- Explain why this enhancement would be useful
- List some examples of how it would be used

### Pull Requests

- Fill in the required template
- Follow the coding standards
- Include appropriate tests
- Update documentation as needed
- Ensure CI/CD pipeline passes

---

## Development Setup

### 1. Fork and Clone

```bash
# Fork the repository on GitHub
git clone https://github.com/YOUR_USERNAME/ongc_.net_app.git
cd ongc_.net_app
git remote add upstream https://github.com/athulkrizz/ongc_.net_app.git
```

### 2. Create a Branch

```bash
git checkout -b feature/your-feature-name
# or
git checkout -b fix/your-bug-fix
```

### 3. Setup Development Environment

**Install Dependencies:**

```bash
# .NET API
cd ONGC.MilestoneAPI
dotnet restore
dotnet build

# Node.js Consumer
cd ../milestone-event-consumer
npm install
```

**Setup Database:**

```bash
# .NET migrations
cd ONGC.MilestoneAPI
dotnet ef database update

# Node.js migrations
cd ../milestone-event-consumer
npm run migrate
```

### 4. Make Your Changes

- Write clean, readable code
- Follow the existing code style
- Add tests for new features
- Update documentation

### 5. Test Your Changes

```bash
# .NET tests
cd ONGC.MilestoneAPI.Tests
dotnet test

# Node.js tests
cd milestone-event-consumer
npm test

# Integration tests
# Run through the complete flow using Postman collection
```

---

## Pull Request Process

### 1. Update Your Branch

```bash
git fetch upstream
git rebase upstream/main
```

### 2. Run Tests

```bash
# Ensure all tests pass
dotnet test
npm test
```

### 3. Commit Your Changes

```bash
git add .
git commit -m "feat: add amazing feature"
```

### 4. Push to Your Fork

```bash
git push origin feature/your-feature-name
```

### 5. Create Pull Request

1. Go to your fork on GitHub
2. Click "New Pull Request"
3. Fill in the PR template
4. Link any related issues
5. Request review

### 6. Address Review Comments

- Respond to all comments
- Make requested changes
- Push updates to the same branch
- Request re-review

---

## Coding Standards

### .NET (C#)

**Follow Microsoft C# Coding Conventions:**

```csharp
// Good
public class MilestoneService : IMilestoneService
{
	private readonly IMilestoneRepository _repository;
	private readonly ILogger<MilestoneService> _logger;

	public MilestoneService(
		IMilestoneRepository repository,
		ILogger<MilestoneService> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<Milestone> CreateAsync(MilestoneCreateDto dto)
	{
		// Implementation
	}
}
```

**Guidelines:**
- Use PascalCase for classes, methods, and properties
- Use camelCase for local variables and parameters
- Prefix private fields with underscore `_`
- Use async/await for asynchronous operations
- Add XML documentation for public APIs
- Keep methods small and focused

### TypeScript/Node.js

**Follow Airbnb TypeScript Style Guide:**

```typescript
// Good
export class EventProcessor {
  private readonly logger: Logger;
  private readonly dbService: DatabaseService;

  constructor(dbService: DatabaseService, logger: Logger) {
	this.dbService = dbService;
	this.logger = logger;
  }

  async processEvent(event: MilestoneEvent): Promise<void> {
	// Implementation
  }
}
```

**Guidelines:**
- Use camelCase for variables and functions
- Use PascalCase for classes and interfaces
- Use UPPER_CASE for constants
- Add JSDoc comments for public APIs
- Use async/await over promises
- Avoid `any` type, use proper typing

### General Guidelines

- **DRY**: Don't Repeat Yourself
- **SOLID**: Follow SOLID principles
- **KISS**: Keep It Simple, Stupid
- **YAGNI**: You Aren't Gonna Need It
- Write self-documenting code
- Comment only when necessary
- Prefer composition over inheritance

---

## Commit Message Guidelines

Follow [Conventional Commits](https://www.conventionalcommits.org/) specification.

### Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types

- **feat**: A new feature
- **fix**: A bug fix
- **docs**: Documentation only changes
- **style**: Code style changes (formatting, missing semi-colons, etc.)
- **refactor**: Code refactoring
- **perf**: Performance improvements
- **test**: Adding or updating tests
- **chore**: Changes to build process or auxiliary tools
- **ci**: CI/CD pipeline changes

### Examples

```bash
feat(api): add milestone filtering endpoint

Add GET /api/Milestone/filter endpoint that allows filtering
by asset, well, and date range.

Closes #123

---

fix(consumer): handle null event data gracefully

Consumer was crashing when receiving null event data.
Added validation to check for null before processing.

Fixes #456

---

docs(readme): update installation instructions

Added Docker installation steps and improved clarity
of setup process.

---

refactor(api): extract validation logic to helper

Moved validation logic from controller to separate
ValidationHelper class for better reusability.

---

test(consumer): add tests for event processor

Added unit tests for EventProcessor covering
success and error scenarios.
```

---

## Testing Guidelines

### Unit Tests

**Write tests for:**
- Business logic
- Data validation
- Error handling
- Edge cases

**.NET Example:**

```csharp
[Fact]
public async Task CreateAsync_WithValidData_ReturnsCreatedMilestone()
{
	// Arrange
	var dto = new MilestoneCreateDto { /* ... */ };
	var service = new MilestoneService(mockRepo, mockLogger);

	// Act
	var result = await service.CreateAsync(dto);

	// Assert
	Assert.NotNull(result);
	Assert.Equal(dto.Asset, result.Asset);
}
```

**TypeScript Example:**

```typescript
describe('EventProcessor', () => {
  it('should process valid event successfully', async () => {
	// Arrange
	const event = createMockEvent();
	const processor = new EventProcessor(mockDb, mockLogger);

	// Act
	await processor.processEvent(event);

	// Assert
	expect(mockDb.insert).toHaveBeenCalledWith(event);
  });
});
```

### Integration Tests

Test the complete flow:
1. API receives request
2. Event published to Kafka
3. Consumer processes event
4. Data saved to database

### Test Coverage

- Aim for at least 80% code coverage
- Focus on critical paths
- Don't test for the sake of coverage

---

## Documentation

### When to Update Documentation

Update documentation when you:
- Add a new feature
- Change existing behavior
- Fix a bug that affects usage
- Update configuration options
- Modify API endpoints

### What to Document

- **README.md**: Overview, setup, usage
- **API Docs**: Endpoint specifications
- **Code Comments**: Complex logic
- **CHANGELOG.md**: Version changes

---

## Review Process

### What We Look For

- **Code Quality**: Clean, readable, maintainable
- **Tests**: Adequate test coverage
- **Documentation**: Updated relevant docs
- **Performance**: No performance regressions
- **Security**: No security vulnerabilities

### Timeline

- Initial review: Within 3-5 business days
- Follow-up reviews: Within 2-3 business days
- Merge: After approval from at least one maintainer

---

## Recognition

Contributors will be:
- Listed in the README
- Mentioned in release notes
- Invited to become maintainers (for significant contributions)

---

## Questions?

- **GitHub Issues**: For feature requests and bugs
- **GitHub Discussions**: For questions and ideas
- **Email**: Create an issue for contact information

---

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for your contribution! 🎉
