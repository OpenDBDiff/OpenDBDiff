## Read and complete the full issue template

**Do you want to request a *feature* or report a *bug*?**
- [x] Bug
- [ ] Feature

**Version of OpenDBDiff**

e.g. 0.10.0.0

**What is the current behavior?**

Complete this.

**What is the expected behavior or new feature?**

Complete this.

**Did this work in previous versions of our tool?  If so, in which versions?**

Yes/No/v0.XX

## Reproducibility
Please attach schema generation scripts for the relevant database objects. If your bug report is about changing existing entities (i.e. not creating or dropping an object), please attach the scheme generation scripts of the object before and after the change. This will enable the developers to reproduce your problem easily. Highlight particular areas of interest.

### Example of what you should enter:

#### Before schema change:
```sql
CREATE TYPE [dbo].[MyTableType] AS TABLE(
	[Column1] [varchar](30) NOT NULL,
	[Column2] [date] NOT NULL,
	[Column3] [varchar](5) NOT NULL -- This column will be dropped
)
```


#### After schema change:
```sql
CREATE TYPE [dbo].[MyTableType] AS TABLE(
	[Column1] [varchar](30) NOT NULL,
	[NewColumn] [datetime] NOT NULL, -- This column was added
	[Column2] [date] NULL -- This column now allows NULL values
)
```


