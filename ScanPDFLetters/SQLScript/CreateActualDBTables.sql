/****** Object:  Table [Validate].[RaSQLActualLetter]    Script Date: 06/09/2019 14:01:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Validate].[RaSQLActualLetter](
	[RaSQLActualLetterID] [int] IDENTITY(1,1) NOT NULL,
	[BatchNo] [smallint] NULL,
	[FilePath] [varchar](8000) NULL,
	[FileName] [varchar](500) NULL,
	[DatabaseName] [varchar](30) NULL,
	[FinancialYear] [smallint] NULL,
	[CreationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[RaSQLActualLetterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Validate].[RaSQLActualLetter] ADD  DEFAULT (sysdatetime()) FOR [CreationDate]
GO

CREATE TABLE [Validate].[RaSQLActualLetterHeader](
	[RaSQLActualLetterHeaderID] [int] IDENTITY(1,1) NOT NULL,
	[RaSQLActualLetterID] [int] NOT NULL,
	[CustomerName] [varchar](100) NULL,
	[PropertyRef] [varchar](30) NULL,
	[PaymentRef] [varchar](30) NULL,
	[AreaActualCostTotal] [money] NULL,
	[YourActualCostTotal] [money] NULL,
	[YourOriginalEstimateTotal] [money] NULL,
	[OriginalEstimationDelta] [money] NULL,
	[SinkingFundDate] [varchar](100) NULL,
	[SinkingFundContributionTotal] [money] NULL,
	[SinkingFundInterestTotal] [money] NULL,
	[SinkingFundReplacementCostTotal] [money] NULL,
	[SinkingFundTotal] [money] NULL,
	[ServiceChargeStatementDate] [varchar](50) NULL,
	[HeaderDate] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[RaSQLActualLetterHeaderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [Validate].[RaSQLActualLetterServiceCharge](
	[RaSQLActualLetterServiceChargeID] [int] IDENTITY(1,1) NOT NULL,
	[RaSQLActualLetterID] [int] NOT NULL,
	[RaSQLActualLetterHeaderID] [int] NOT NULL,
	[PropertyRef] [varchar](30) NULL,
	[ServiceName] [varchar](50) NULL,
	[ServiceCategory] [varchar](50) NULL,
	[AreaActualCost] [money] NULL,
	[YourActualCost] [money] NULL,
	[YourOriginalEstimate] [money] NULL,
	[ServiceChargeType] [varchar](20) NOT NULL,
 CONSTRAINT [PK_RaSQLActualLetterServiceCharge] PRIMARY KEY CLUSTERED 
(
	[RaSQLActualLetterServiceChargeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

