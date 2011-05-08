[Stores SVN passwords as plain text in a cookie file. This tool is inteneded to help me automate some merging I have to do at work]

A Web UI for showing revisions that can be merged between two SVN reprositorys and offering to merged selected revisions.
Is running the following commands:

1. Svn MergeInfo <ReproA> <ReproB>
2. Svn Log <Revisions 1st Command Returned>
3. Svn Merge <Stuff user selects from the 2nd command>