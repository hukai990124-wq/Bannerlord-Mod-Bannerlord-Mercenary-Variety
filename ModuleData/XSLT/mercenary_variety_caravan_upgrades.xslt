<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <!-- Connect standard caravan guards to their veteran counterparts. -->
  <xsl:template match="/NPCCharacters[1]/NPCCharacter[starts-with(@id, 'caravan_guard_')]">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()[not(self::upgrade_targets)]" />
      <upgrade_targets>
        <upgrade_target>
          <xsl:attribute name="id">
            <xsl:text>NPCCharacter.veteran_</xsl:text>
            <xsl:value-of select="@id" />
          </xsl:attribute>
        </upgrade_target>
      </upgrade_targets>
    </xsl:copy>
  </xsl:template>

  <!-- Connect NavalDLC caravan guards to their naval veteran counterparts. -->
  <xsl:template match="/NPCCharacters[1]/NPCCharacter[starts-with(@id, 'naval_caravan_guard_')]">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()[not(self::upgrade_targets)]" />
      <upgrade_targets>
        <upgrade_target>
          <xsl:attribute name="id">
            <xsl:text>NPCCharacter.naval_veteran_</xsl:text>
            <xsl:value-of select="substring-after(@id, 'naval_')" />
          </xsl:attribute>
        </upgrade_target>
      </upgrade_targets>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
