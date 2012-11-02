var x = {
    "resourcePath": "/words",
    "apis": [
       {
           "path": "/words.{format}/search/{query}",
           "description": "",
           "operations": [
              {
                  "parameters": [
                     {
                         "name": "query",
                         "description": "Search query",
                         "required": true,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "path"
                     },
                     {
                         "name": "caseSensitive",
                         "defaultValue": "true",
                         "description": "Search case sensitive",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "true",
                                "false"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "includePartOfSpeech",
                         "description": "Only include these comma-delimited parts of speech",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "noun",
                                "adjective",
                                "verb",
                                "adverb",
                                "interjection",
                                "pronoun",
                                "preposition",
                                "abbreviation",
                                "affix",
                                "article",
                                "auxiliary-verb",
                                "conjunction",
                                "definite-article",
                                "family-name",
                                "given-name",
                                "idiom",
                                "imperative",
                                "noun-plural",
                                "noun-posessive",
                                "past-participle",
                                "phrasal-prefix",
                                "proper-noun",
                                "proper-noun-plural",
                                "proper-noun-posessive",
                                "suffix",
                                "verb-intransitive",
                                "verb-transitive"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "excludePartOfSpeech",
                         "description": "Exclude these comma-delimited parts of speech",
                         "required": false,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minCorpusCount",
                         "defaultValue": "5",
                         "description": "Minimum corpus frequency for terms",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": "Infinity",
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxCorpusCount",
                         "defaultValue": "-1",
                         "description": "Maximum corpus frequency for terms",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": "Infinity",
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minDictionaryCount",
                         "defaultValue": "1",
                         "description": "Minimum number of dictionary entries for words returned",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": "Infinity",
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxDictionaryCount",
                         "defaultValue": "-1",
                         "description": "Maximum dictionary definition count",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": "Infinity",
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minLength",
                         "defaultValue": "1",
                         "description": "Minimum word length",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1024.0,
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxLength",
                         "defaultValue": "-1",
                         "description": "Maximum word length",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1024.0,
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "skip",
                         "defaultValue": "0",
                         "description": "Results to skip",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1000.0,
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "limit",
                         "defaultValue": "10",
                         "description": "Maximum number of results to return",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1000.0,
                             "min": 1.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     }
                  ],
                  "summary": "Searches words",
                  "httpMethod": "GET",
                  "responseTypeInternal": "com.wordnik.index.word.WordSearchResults",
                  "errorResponses": [
                     {
                         "reason": "Invalid query supplied.",
                         "code": 400
                     }
                  ],
                  "nickname": "searchWords",
                  "responseClass": "wordSearchResults"
              }
           ]
       },
       {
           "path": "/words.{format}/wordOfTheDay",
           "description": "",
           "operations": [
              {
                  "parameters": [
                     {
                         "name": "date",
                         "description": "Fetches by date in yyyy-MM-dd",
                         "required": false,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     }
                  ],
                  "summary": "Returns a specific WordOfTheDay",
                  "httpMethod": "GET",
                  "responseTypeInternal": "com.wordnik.community.entity.WordOfTheDay",
                  "nickname": "getWordOfTheDay",
                  "responseClass": "WordOfTheDay"
              }
           ]
       },
       {
           "path": "/words.{format}/reverseDictionary",
           "description": "",
           "operations": [
              {
                  "parameters": [
                     {
                         "name": "query",
                         "description": "Search term",
                         "required": true,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "findSenseForWord",
                         "description": "Restricts words and finds closest sense",
                         "required": false,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "includeSourceDictionaries",
                         "description": "Only include these comma-delimited source dictionaries",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "ahd",
                                " century",
                                " wiktionary",
                                " webster",
                                " wordnet"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "excludeSourceDictionaries",
                         "description": "Exclude these comma-delimited source dictionaries",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "ahd",
                                " century",
                                " wiktionary",
                                " webster",
                                " wordnet"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "includePartOfSpeech",
                         "description": "Only include these comma-delimited parts of speech",
                         "required": false,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "excludePartOfSpeech",
                         "description": "Exclude these comma-delimited parts of speech",
                         "required": false,
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minCorpusCount",
                         "defaultValue": "5",
                         "description": "Minimum corpus frequency for terms",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": "Infinity",
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxCorpusCount",
                         "defaultValue": "-1",
                         "description": "Maximum corpus frequency for terms",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": "Infinity",
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minLength",
                         "defaultValue": "1",
                         "description": "Minimum word length",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1024.0,
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxLength",
                         "defaultValue": "-1",
                         "description": "Maximum word length",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1024.0,
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "expandTerms",
                         "description": "Expand terms",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "synonym",
                                "hypernym"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "includeTags",
                         "defaultValue": "false",
                         "description": "Return a closed set of XML tags in response",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "false",
                                "true"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "sortBy",
                         "description": "Attribute to sort by",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "alpha",
                                "count",
                                "length"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "sortOrder",
                         "description": "Sort direction",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "asc",
                                "desc"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "skip",
                         "defaultValue": "0",
                         "description": "Results to skip",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1000.0,
                             "min": 0.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "limit",
                         "defaultValue": "10",
                         "description": "Maximum number of results to return",
                         "required": false,
                         "allowableValues": {
                             "valueType": "RANGE",
                             "max": 1000.0,
                             "min": 1.0,
                             "valueType": "RANGE"
                         },
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     }
                  ],
                  "summary": "Reverse dictionary search",
                  "httpMethod": "GET",
                  "responseTypeInternal": "com.wordnik.index.definition.DefinitionSearchResults",
                  "errorResponses": [
                     {
                         "reason": "Invalid term supplied.",
                         "code": 400
                     }
                  ],
                  "nickname": "reverseDictionary",
                  "responseClass": "DefinitionSearchResults"
              }
           ]
       },
       {
           "path": "/words.{format}/randomWords",
           "description": "",
           "operations": [
              {
                  "parameters": [
                     {
                         "name": "hasDictionaryDef",
                         "defaultValue": "true",
                         "description": "Only return words with dictionary definitions",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "false",
                                "true"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "includePartOfSpeech",
                         "description": "CSV part-of-speech values to include",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "noun",
                                "adjective",
                                "verb",
                                "adverb",
                                "interjection",
                                "pronoun",
                                "preposition",
                                "abbreviation",
                                "affix",
                                "article",
                                "auxiliary-verb",
                                "conjunction",
                                "definite-article",
                                "family-name",
                                "given-name",
                                "idiom",
                                "imperative",
                                "noun-plural",
                                "noun-posessive",
                                "past-participle",
                                "phrasal-prefix",
                                "proper-noun",
                                "proper-noun-plural",
                                "proper-noun-posessive",
                                "suffix",
                                "verb-intransitive",
                                "verb-transitive"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "excludePartOfSpeech",
                         "description": "CSV part-of-speech values to exclude",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "noun",
                                "adjective",
                                "verb",
                                "adverb",
                                "interjection",
                                "pronoun",
                                "preposition",
                                "abbreviation",
                                "affix",
                                "article",
                                "auxiliary-verb",
                                "conjunction",
                                "definite-article",
                                "family-name",
                                "given-name",
                                "idiom",
                                "imperative",
                                "noun-plural",
                                "noun-posessive",
                                "past-participle",
                                "phrasal-prefix",
                                "proper-noun",
                                "proper-noun-plural",
                                "proper-noun-posessive",
                                "suffix",
                                "verb-intransitive",
                                "verb-transitive"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minCorpusCount",
                         "defaultValue": "0",
                         "description": "Minimum corpus frequency for terms",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxCorpusCount",
                         "defaultValue": "-1",
                         "description": "Maximum corpus frequency for terms",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minDictionaryCount",
                         "defaultValue": "1",
                         "description": "Minimum dictionary count",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxDictionaryCount",
                         "defaultValue": "-1",
                         "description": "Maximum dictionary count",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minLength",
                         "defaultValue": "5",
                         "description": "Minimum word length",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxLength",
                         "defaultValue": "-1",
                         "description": "Maximum word length",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "sortBy",
                         "description": "Attribute to sort by",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "alpha",
                                "count"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "sortOrder",
                         "description": "Sort direction",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "asc",
                                "desc"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "limit",
                         "defaultValue": "10",
                         "description": "Maximum number of results to return",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     }
                  ],
                  "summary": "Returns an array of random WordObjects",
                  "httpMethod": "GET",
                  "responseTypeInternal": "com.wordnik.corpus.model.WordWrapper",
                  "errorResponses": [
                     {
                         "reason": "Invalid term supplied.",
                         "code": 400
                     },
                     {
                         "reason": "No results.",
                         "code": 404
                     }
                  ],
                  "nickname": "getRandomWords",
                  "responseClass": "List[wordObject]"
              }
           ]
       },
       {
           "path": "/words.{format}/randomWord",
           "description": "",
           "operations": [
              {
                  "parameters": [
                     {
                         "name": "hasDictionaryDef",
                         "defaultValue": "true",
                         "description": "Only return words with dictionary definitions",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "false",
                                "true"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "includePartOfSpeech",
                         "description": "CSV part-of-speech values to include",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "noun",
                                "adjective",
                                "verb",
                                "adverb",
                                "interjection",
                                "pronoun",
                                "preposition",
                                "abbreviation",
                                "affix",
                                "article",
                                "auxiliary-verb",
                                "conjunction",
                                "definite-article",
                                "family-name",
                                "given-name",
                                "idiom",
                                "imperative",
                                "noun-plural",
                                "noun-posessive",
                                "past-participle",
                                "phrasal-prefix",
                                "proper-noun",
                                "proper-noun-plural",
                                "proper-noun-posessive",
                                "suffix",
                                "verb-intransitive",
                                "verb-transitive"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "excludePartOfSpeech",
                         "description": "CSV part-of-speech values to exclude",
                         "required": false,
                         "allowableValues": {
                             "valueType": "LIST",
                             "values": [
                                "noun",
                                "adjective",
                                "verb",
                                "adverb",
                                "interjection",
                                "pronoun",
                                "preposition",
                                "abbreviation",
                                "affix",
                                "article",
                                "auxiliary-verb",
                                "conjunction",
                                "definite-article",
                                "family-name",
                                "given-name",
                                "idiom",
                                "imperative",
                                "noun-plural",
                                "noun-posessive",
                                "past-participle",
                                "phrasal-prefix",
                                "proper-noun",
                                "proper-noun-plural",
                                "proper-noun-posessive",
                                "suffix",
                                "verb-intransitive",
                                "verb-transitive"
                             ],
                             "valueType": "LIST"
                         },
                         "dataType": "string",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minCorpusCount",
                         "defaultValue": "0",
                         "description": "Minimum corpus frequency for terms",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxCorpusCount",
                         "defaultValue": "-1",
                         "description": "Maximum corpus frequency for terms",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minDictionaryCount",
                         "defaultValue": "1",
                         "description": "Minimum dictionary count",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxDictionaryCount",
                         "defaultValue": "-1",
                         "description": "Maximum dictionary count",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "minLength",
                         "defaultValue": "5",
                         "description": "Minimum word length",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     },
                     {
                         "name": "maxLength",
                         "defaultValue": "-1",
                         "description": "Maximum word length",
                         "required": false,
                         "dataType": "int",
                         "allowMultiple": false,
                         "paramType": "query"
                     }
                  ],
                  "summary": "Returns a single random WordObject",
                  "httpMethod": "GET",
                  "responseTypeInternal": "com.wordnik.corpus.model.WordWrapper",
                  "errorResponses": [
                     {
                         "reason": "No word found.",
                         "code": 404
                     }
                  ],
                  "nickname": "getRandomWord",
                  "responseClass": "wordObject"
              }
           ]
       }
    ],
    "apiVersion": "4.0",
    "swaggerVersion": "1.0",
    "basePath": "http://api.wordnik.com/v4",
    "models": {
        "WordSearchResult": {
            "properties": {
                "count": {
                    "type": "long"
                },
                "lexicality": {
                    "type": "double"
                },
                "word": {
                    "type": "string"
                }
            },
            "id": "WordSearchResult"
        },
        "WordOfTheDay": {
            "properties": {
                "id": {
                    "type": "long",
                    "required": true
                },
                "parentId": {
                    "type": "string"
                },
                "category": {
                    "type": "string"
                },
                "createdBy": {
                    "type": "string"
                },
                "createdAt": {
                    "type": "Date"
                },
                "contentProvider": {
                    "type": "contentProvider"
                },
                "htmlExtra": {
                    "type": "string"
                },
                "word": {
                    "type": "string"
                },
                "definitions": {
                    "type": "array",
                    "items": {
                        "$ref": "simpleDefinition"
                    }
                },
                "examples": {
                    "type": "array",
                    "items": {
                        "$ref": "simpleExample"
                    }
                },
                "note": {
                    "type": "string"
                },
                "publishDate": {
                    "type": "Date"
                }
            },
            "id": "WordOfTheDay"
        },
        "Note": {
            "properties": {
                "noteType": {
                    "type": "string"
                },
                "appliesTo": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "value": {
                    "type": "string"
                },
                "pos": {
                    "type": "int"
                }
            },
            "id": "note"
        },
        "DefinitionSearchResults": {
            "properties": {
                "results": {
                    "type": "array",
                    "items": {
                        "$ref": "definition"
                    }
                },
                "totalResults": {
                    "type": "int"
                }
            },
            "id": "DefinitionSearchResults"
        },
        "WordObject": {
            "properties": {
                "id": {
                    "type": "long",
                    "required": true
                },
                "word": {
                    "type": "string"
                },
                "originalWord": {
                    "type": "string"
                },
                "suggestions": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "canonicalForm": {
                    "type": "string"
                },
                "vulgar": {
                    "type": "string"
                }
            },
            "id": "wordObject"
        },
        "Related": {
            "properties": {
                "label1": {
                    "type": "string"
                },
                "relationshipType": {
                    "type": "string"
                },
                "label2": {
                    "type": "string"
                },
                "label3": {
                    "type": "string"
                },
                "words": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "gram": {
                    "type": "string"
                },
                "label4": {
                    "type": "string"
                }
            },
            "id": "related"
        },
        "Citation": {
            "properties": {
                "cite": {
                    "type": "string"
                },
                "source": {
                    "type": "string"
                }
            },
            "id": "citation"
        },
        "WordSearchResults": {
            "properties": {
                "searchResults": {
                    "type": "array",
                    "items": {
                        "$ref": "WordSearchResult"
                    }
                },
                "totalResults": {
                    "type": "int"
                }
            },
            "id": "wordSearchResults"
        },
        "Category": {
            "properties": {
                "id": {
                    "type": "long",
                    "required": true
                },
                "name": {
                    "type": "string"
                }
            },
            "id": "category"
        },
        "SimpleDefinition": {
            "properties": {
                "text": {
                    "type": "string"
                },
                "source": {
                    "type": "string"
                },
                "note": {
                    "type": "string"
                },
                "partOfSpeech": {
                    "type": "string"
                }
            },
            "id": "simpleDefinition"
        },
        "Root": {
            "properties": {
                "id": {
                    "type": "long",
                    "required": true
                },
                "name": {
                    "type": "string"
                },
                "categories": {
                    "type": "array",
                    "items": {
                        "$ref": "category"
                    }
                }
            },
            "id": "root"
        },
        "ExampleUsage": {
            "properties": {
                "text": {
                    "type": "string"
                }
            },
            "id": "ExampleUsage"
        },
        "ContentProvider": {
            "properties": {
                "id": {
                    "type": "int"
                },
                "name": {
                    "type": "string"
                }
            },
            "id": "contentProvider"
        },
        "Label": {
            "properties": {
                "text": {
                    "type": "string"
                },
                "type": {
                    "type": "string"
                }
            },
            "id": "Label"
        },
        "SimpleExample": {
            "properties": {
                "id": {
                    "type": "long"
                },
                "title": {
                    "type": "string"
                },
                "text": {
                    "type": "string"
                },
                "url": {
                    "type": "string"
                }
            },
            "id": "simpleExample"
        },
        "Definition": {
            "properties": {
                "extendedText": {
                    "type": "string"
                },
                "text": {
                    "type": "string"
                },
                "sourceDictionary": {
                    "type": "string"
                },
                "citations": {
                    "type": "array",
                    "items": {
                        "$ref": "citation"
                    }
                },
                "labels": {
                    "type": "array",
                    "items": {
                        "$ref": "Label"
                    }
                },
                "score": {
                    "type": "float"
                },
                "exampleUses": {
                    "type": "array",
                    "items": {
                        "$ref": "ExampleUsage"
                    }
                },
                "attributionUrl": {
                    "type": "string"
                },
                "seqString": {
                    "type": "string"
                },
                "attributionText": {
                    "type": "string"
                },
                "relatedWords": {
                    "type": "array",
                    "items": {
                        "$ref": "related"
                    }
                },
                "sequence": {
                    "type": "string"
                },
                "word": {
                    "type": "string"
                },
                "notes": {
                    "type": "array",
                    "items": {
                        "$ref": "note"
                    }
                },
                "textProns": {
                    "type": "array",
                    "items": {
                        "$ref": "textPron"
                    }
                },
                "partOfSpeech": {
                    "type": "string"
                }
            },
            "id": "definition"
        },
        "PartOfSpeech": {
            "properties": {
                "roots": {
                    "type": "array",
                    "items": {
                        "$ref": "root"
                    }
                },
                "storageAbbr": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                },
                "allCategories": {
                    "type": "array",
                    "items": {
                        "$ref": "category"
                    }
                }
            },
            "id": "partOfSpeech"
        },
        "TextPron": {
            "properties": {
                "raw": {
                    "type": "string"
                },
                "seq": {
                    "type": "int"
                },
                "rawType": {
                    "type": "string"
                }
            },
            "id": "textPron"
        }
    }
}