
/****** Object:  Table [Validate].[RaSQLHeaderLetters]    Script Date: 05/09/2019 16:39:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Validate].[RaSQLHeaderLetters](
	[RaSQLLetterID] [int] NULL,
	[PropertyRef] [varchar](30) NULL,
	[RentsTotal] [money] NULL,
	[ServicesTotal] [money] NULL,
	[PrivateTotal] [money] NULL,
	[RentServiceTotal] [money] NULL,
	[HeaderDate] [varchar](50) NULL,
	[StatementDate] [varchar](50) NULL
) ON [PRIMARY]
GO


/****** Object:  Table [Validate].[RaSQLLetters]    Script Date: 05/09/2019 16:39:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Validate].[RaSQLLetters](
	[RaSQLLetterID] [int] IDENTITY(1,1) NOT NULL,
	[BatchNo] [smallint] NULL,
	[FilePath] [varchar](8000) NULL,
	[FileName] [varchar](500) NULL,
	[DatabaseName] [varchar](30) NULL,
	[FinancialYear] [smallint] NULL,
	[CreationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RaSQLLetterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Validate].[RaSQLLetters] ADD  DEFAULT (sysdatetime()) FOR [CreationDate]
GO


/****** Object:  Table [Validate].[RaSQLServicesLetter]    Script Date: 05/09/2019 16:39:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Validate].[RaSQLServicesLetter](
	[RaSQLLetterID] [int] NULL,
	[PropertyRef] [varchar](30) NULL,
	[Service] [varchar](50) NULL,
	[AreaEstimatedCost] [money] NULL,
	[YourEstimatedCost] [money] NULL
) ON [PRIMARY]
GO

