CREATE TABLE [dbo].[ExperiencesXInstructors] (
    [Experience_id] UNIQUEIDENTIFIER NOT NULL,
    [Instructor_id] UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Experience_id] ASC, [Instructor_id] ASC),
    CONSTRAINT [FK2D59B60671B6F7C9] FOREIGN KEY ([Instructor_id]) REFERENCES [dbo].[Instructors] ([ID]),
    CONSTRAINT [FK2D59B606DD4F580E] FOREIGN KEY ([Experience_id]) REFERENCES [dbo].[Experiences] ([ID])
);

