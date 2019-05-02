# https://stackoverflow.com/questions/17140886/how-to-search-and-replace-text-in-a-file-using-python/17141572

# Read in the file
with open('C:/Users/me/Documents/raw.txt', 'r') as file :
  filedata = file.read()

# replace whatever string you want to replace
for i in range(0, 400):
    filedata = filedata.replace('old-string', 'new-string')

# Write the file out again
with open('C:/Users/me/Documents/output.txt', 'w') as file:
  file.write(filedata)
