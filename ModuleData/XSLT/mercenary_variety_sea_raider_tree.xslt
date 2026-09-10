<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output omit-xml-declaration="yes" indent="yes" />

  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <!-- Keep vanilla / NavalDLC regular-army off-ramps. Re-add the sea raider spine
       when War Sails has replaced upgrade_targets with Nord troops only. -->
  <xsl:template match="NPCCharacter[@id='sea_raiders_bandit']/upgrade_targets">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <xsl:if test="not(upgrade_target[@id='NPCCharacter.sea_raiders_raider'])">
        <upgrade_target id="NPCCharacter.sea_raiders_raider" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sea_raiders_raider']/upgrade_targets">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <xsl:if test="not(upgrade_target[@id='NPCCharacter.sea_raiders_chief'])">
        <upgrade_target id="NPCCharacter.sea_raiders_chief" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sea_raiders_chief']/upgrade_targets">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <xsl:if test="not(upgrade_target[@id='NPCCharacter.sea_raiders_boss'])">
        <upgrade_target id="NPCCharacter.sea_raiders_boss" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sea_raiders_boss']/upgrade_targets">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <xsl:if test="not(upgrade_target[@id='NPCCharacter.mv_sea_raider_warlord'])">
        <upgrade_target id="NPCCharacter.mv_sea_raider_warlord" />
      </xsl:if>
    </xsl:copy>
  </xsl:template>

  <xsl:template match="NPCCharacter[@id='sea_raiders_boss'][not(upgrade_targets)]">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
      <upgrade_targets>
        <upgrade_target id="NPCCharacter.mv_sea_raider_warlord" />
      </upgrade_targets>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
